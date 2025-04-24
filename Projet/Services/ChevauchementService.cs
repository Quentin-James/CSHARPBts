using Microsoft.Data.SqlClient;
using Services.DTOs;
using Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text;
using System.IO;
using System.Linq;

namespace Services
{
    public class ChevauchementService : IChevauchementService
    {
        private readonly string _connectionString;
        private readonly ILogger<ChevauchementService> _logger;
        private const int TITRE_MAX_LENGTH = 100;

        private async Task UpdateTitreColumnLengthAsync(SqlConnection connection)
        {
            try
            {
                var alterCommand = new SqlCommand(
                    "ALTER TABLE Spectacles ALTER COLUMN Titre NVARCHAR(100)",
                    connection);
                
                await alterCommand.ExecuteNonQueryAsync();
                _logger.LogInformation("Taille de la colonne Titre mise à jour avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "La modification de la taille de la colonne Titre a échoué. Cela peut être normal si la taille est déjà correcte.");
            }
        }

        public ChevauchementService(IConfiguration configuration, ILogger<ChevauchementService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string not found");
            _logger = logger;
        }

        private string TruncateTitre(string titre)
        {
            if (string.IsNullOrEmpty(titre)) return titre;
            return titre.Length <= TITRE_MAX_LENGTH ? titre : titre.Substring(0, TITRE_MAX_LENGTH);
        }

        private void ValidateRequiredFields(string titre, string description, string type, string duree, string saison)
        {
            if (string.IsNullOrWhiteSpace(titre))
                throw new ArgumentException("Le titre est obligatoire");
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("La description est obligatoire");
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Le type est obligatoire");
            if (string.IsNullOrWhiteSpace(duree))
                throw new ArgumentException("La durée est obligatoire");
            if (string.IsNullOrWhiteSpace(saison))
                throw new ArgumentException("La saison est obligatoire");
        }

        public async Task<List<string>> GetTableNamesAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new SqlCommand(
                    "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'",
                    connection);

                var tables = new List<string>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tables.Add(reader.GetString(0));
                }

                _logger.LogInformation($"Tables trouvées: {string.Join(", ", tables)}");
                return tables;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des noms de tables");
                throw;
            }
        }

        public async Task<List<ChevauchementDto>> GetChevauchementsAsync()
        {
            try
            {
                _logger.LogInformation("Début de la recherche des chevauchements");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Vérifier s'il y a des données dans les tables
                var checkCommand = new SqlCommand(
                    "SELECT COUNT(*) FROM Spectacles; SELECT COUNT(*) FROM Programmation",
                    connection);

                using var reader = await checkCommand.ExecuteReaderAsync();
                await reader.ReadAsync();
                var spectaclesCount = reader.GetInt32(0);
                await reader.NextResultAsync();
                await reader.ReadAsync();
                var programmationsCount = reader.GetInt32(0);
                reader.Close();

                _logger.LogInformation($"Nombre de spectacles: {spectaclesCount}, Nombre de programmations: {programmationsCount}");

                if (spectaclesCount == 0 || programmationsCount == 0)
                {
                    _logger.LogInformation("Aucune donnée trouvée dans les tables");
                    return new List<ChevauchementDto>();
                }

                var sql = @"
                    SELECT DISTINCT
                        p1.SpectacleId,
                        s1.Titre,
                        p1.Date,
                        p1.Heure as HeureDebut,
                        DATEADD(MINUTE, DATEDIFF(MINUTE, '00:00', s1.Duree), p1.Heure) as HeureFin,
                        p2.SpectacleId as SpectacleChevaucheId,
                        s2.Titre as TitreChevauchement,
                        p2.Heure as HeureDebutChevauchement,
                        DATEADD(MINUTE, DATEDIFF(MINUTE, '00:00', s2.Duree), p2.Heure) as HeureFinChevauchement
                    FROM 
                        Programmation p1
                    JOIN 
                        Spectacles s1 ON p1.SpectacleId = s1.SpectacleId
                    JOIN 
                        Programmation p2 ON p1.Date = p2.Date
                    JOIN 
                        Spectacles s2 ON p2.SpectacleId = s2.SpectacleId
                    WHERE 
                        p1.SpectacleId < p2.SpectacleId
                        AND (
                            (p1.Heure >= p2.Heure AND p1.Heure < DATEADD(MINUTE, DATEDIFF(MINUTE, '00:00', s2.Duree), p2.Heure))
                            OR 
                            (p2.Heure >= p1.Heure AND p2.Heure < DATEADD(MINUTE, DATEDIFF(MINUTE, '00:00', s1.Duree), p1.Heure))
                        )
                    ORDER BY 
                        p1.Date, p1.Heure";

                _logger.LogInformation("Exécution de la requête SQL");
                var command = new SqlCommand(sql, connection);
                var chevauchements = new List<ChevauchementDto>();

                using var resultReader = await command.ExecuteReaderAsync();
                while (await resultReader.ReadAsync())
                {
                    chevauchements.Add(new ChevauchementDto
                    {
                        SpectacleId = resultReader.GetInt32(0),
                        Titre = resultReader.GetString(1),
                        Date = DateOnly.FromDateTime(resultReader.GetDateTime(2)),
                        HeureDebut = TimeOnly.FromTimeSpan(resultReader.GetTimeSpan(3)),
                        HeureFin = TimeOnly.FromTimeSpan(resultReader.GetTimeSpan(4)),
                        SpectacleChevaucheId = resultReader.GetInt32(5),
                        TitreChevauchement = resultReader.GetString(6),
                        HeureDebutChevauchement = TimeOnly.FromTimeSpan(resultReader.GetTimeSpan(7)),
                        HeureFinChevauchement = TimeOnly.FromTimeSpan(resultReader.GetTimeSpan(8))
                    });
                }

                _logger.LogInformation($"Nombre de chevauchements trouvés: {chevauchements.Count}");
                return chevauchements;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche des chevauchements");
                throw new Exception($"Erreur lors de la recherche des chevauchements: {ex.Message}", ex);
            }
        }

        public async Task InsertTestDataAsync()
        {
            try
            {
                _logger.LogInformation("Début de l'insertion des données de test");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Insérer les spectacles
                var insertSpectacleCommand = new SqlCommand(
                    @"INSERT INTO Spectacles (
                        Titre, 
                        Description, 
                        Duree, 
                        Type, 
                        Saison, 
                        DeconseilleAuxEnfants,
                        SpectacleEnfant1Id,
                        SpectacleEnfant2Id,
                        SpectacleEnfant3Id
                    ) VALUES (
                        @Titre, 
                        @Description, 
                        @Duree, 
                        @Type, 
                        @Saison, 
                        @DeconseilleAuxEnfants,
                        @SpectacleEnfant1Id,
                        @SpectacleEnfant2Id,
                        @SpectacleEnfant3Id
                    ); SELECT SCOPE_IDENTITY();",
                    connection);

                // Premier spectacle
                var titre1 = TruncateTitre("Spectacle Test 1");
                insertSpectacleCommand.Parameters.AddWithValue("@Titre", titre1);
                insertSpectacleCommand.Parameters.AddWithValue("@Description", "Description du spectacle test 1");
                insertSpectacleCommand.Parameters.AddWithValue("@Duree", new TimeSpan(2, 0, 0));
                insertSpectacleCommand.Parameters.AddWithValue("@Type", "Standard");
                insertSpectacleCommand.Parameters.AddWithValue("@Saison", "2024");
                insertSpectacleCommand.Parameters.AddWithValue("@DeconseilleAuxEnfants", false);
                insertSpectacleCommand.Parameters.AddWithValue("@SpectacleEnfant1Id", DBNull.Value);
                insertSpectacleCommand.Parameters.AddWithValue("@SpectacleEnfant2Id", DBNull.Value);
                insertSpectacleCommand.Parameters.AddWithValue("@SpectacleEnfant3Id", DBNull.Value);
                var spectacle1Id = Convert.ToInt32(await insertSpectacleCommand.ExecuteScalarAsync());

                // Deuxième spectacle
                insertSpectacleCommand.Parameters.Clear();
                var titre2 = TruncateTitre("Spectacle Test 2");
                insertSpectacleCommand.Parameters.AddWithValue("@Titre", titre2);
                insertSpectacleCommand.Parameters.AddWithValue("@Description", "Description du spectacle test 2");
                insertSpectacleCommand.Parameters.AddWithValue("@Duree", new TimeSpan(1, 30, 0));
                insertSpectacleCommand.Parameters.AddWithValue("@Type", "Standard");
                insertSpectacleCommand.Parameters.AddWithValue("@Saison", "2024");
                insertSpectacleCommand.Parameters.AddWithValue("@DeconseilleAuxEnfants", false);
                insertSpectacleCommand.Parameters.AddWithValue("@SpectacleEnfant1Id", DBNull.Value);
                insertSpectacleCommand.Parameters.AddWithValue("@SpectacleEnfant2Id", DBNull.Value);
                insertSpectacleCommand.Parameters.AddWithValue("@SpectacleEnfant3Id", DBNull.Value);
                var spectacle2Id = Convert.ToInt32(await insertSpectacleCommand.ExecuteScalarAsync());

                _logger.LogInformation("Spectacles de test créés");

                // Insérer les programmations
                var insertProgrammationCommand = new SqlCommand(
                    "INSERT INTO Programmation (SpectacleId, Date, Heure, Lieu) VALUES (@SpectacleId, @Date, @Heure, @Lieu)",
                    connection);

                // Première programmation
                insertProgrammationCommand.Parameters.AddWithValue("@SpectacleId", spectacle1Id);
                insertProgrammationCommand.Parameters.AddWithValue("@Date", DateOnly.FromDateTime(DateTime.Today));
                insertProgrammationCommand.Parameters.AddWithValue("@Heure", TimeOnly.FromTimeSpan(new TimeSpan(20, 0, 0)));
                insertProgrammationCommand.Parameters.AddWithValue("@Lieu", "Salle 1");
                await insertProgrammationCommand.ExecuteNonQueryAsync();

                // Deuxième programmation
                insertProgrammationCommand.Parameters.Clear();
                insertProgrammationCommand.Parameters.AddWithValue("@SpectacleId", spectacle2Id);
                insertProgrammationCommand.Parameters.AddWithValue("@Date", DateOnly.FromDateTime(DateTime.Today));
                insertProgrammationCommand.Parameters.AddWithValue("@Heure", TimeOnly.FromTimeSpan(new TimeSpan(21, 0, 0)));
                insertProgrammationCommand.Parameters.AddWithValue("@Lieu", "Salle 2");
                await insertProgrammationCommand.ExecuteNonQueryAsync();

                _logger.LogInformation("Programmations de test créées");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'insertion des données de test");
                throw new Exception($"Erreur lors de l'insertion des données de test: {ex.Message}", ex);
            }
        }

        private async Task<bool> CheckDuplicateSpectacleAsync(SqlConnection connection, string titre)
        {
            var checkCommand = new SqlCommand(@"
                SELECT COUNT(1) 
                FROM Spectacles 
                WHERE LOWER(Titre) = LOWER(@Titre)", connection);

            checkCommand.Parameters.AddWithValue("@Titre", titre);

            var count = (int)await checkCommand.ExecuteScalarAsync();
            return count > 0;
        }

        private async Task CleanupDuplicatesAsync(SqlConnection connection)
        {
            var cleanupCommand = new SqlCommand(@"
                WITH DuplicateCTE AS (
                    SELECT 
                        SpectacleId,
                        Titre,
                        ROW_NUMBER() OVER (PARTITION BY Titre ORDER BY SpectacleId) as RowNum
                    FROM Spectacles
                )
                DELETE FROM Spectacles 
                WHERE SpectacleId IN (
                    SELECT SpectacleId 
                    FROM DuplicateCTE 
                    WHERE RowNum > 1
                )", connection);

            var deletedRows = await cleanupCommand.ExecuteNonQueryAsync();
            _logger.LogInformation($"Nettoyage des doublons : {deletedRows} enregistrements supprimés");
        }

        private static Encoding GetFileEncoding(string filePath)
        {
            var bom = new byte[4];
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode;
            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode;
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return Encoding.UTF32;

            using (var reader = new StreamReader(filePath, Encoding.Default, true))
            {
                reader.Peek();
                return reader.CurrentEncoding;
            }
        }

        public async Task ImportFromCsvAsync(string filePath)
        {
            try
            {
                _logger.LogInformation($"Début de l'importation depuis le fichier CSV: {filePath}");

                var encoding = GetFileEncoding(filePath);
                using var reader = new StreamReader(filePath, encoding);
                await reader.ReadLineAsync(); // Skip header

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var insertCommand = new SqlCommand(
                    @"MERGE INTO Spectacles AS target
                    USING (VALUES (
                        @Titre, 
                        @Description, 
                        @Duree, 
                        @Type, 
                        @Saison, 
                        @DeconseilleAuxEnfants,
                        @SpectacleEnfant1Id,
                        @SpectacleEnfant2Id,
                        @SpectacleEnfant3Id
                    )) AS source (
                        Titre, 
                        Description, 
                        Duree, 
                        Type, 
                        Saison, 
                        DeconseilleAuxEnfants,
                        SpectacleEnfant1Id,
                        SpectacleEnfant2Id,
                        SpectacleEnfant3Id
                    )
                    ON target.Titre = source.Titre
                    WHEN MATCHED THEN
                        UPDATE SET
                            Description = source.Description,
                            Duree = source.Duree,
                            Type = source.Type,
                            Saison = source.Saison,
                            DeconseilleAuxEnfants = source.DeconseilleAuxEnfants,
                            SpectacleEnfant1Id = source.SpectacleEnfant1Id,
                            SpectacleEnfant2Id = source.SpectacleEnfant2Id,
                            SpectacleEnfant3Id = source.SpectacleEnfant3Id
                    WHEN NOT MATCHED THEN
                        INSERT (
                            Titre, 
                            Description, 
                            Duree, 
                            Type, 
                            Saison, 
                            DeconseilleAuxEnfants,
                            SpectacleEnfant1Id,
                            SpectacleEnfant2Id,
                            SpectacleEnfant3Id
                        )
                        VALUES (
                            source.Titre,
                            source.Description,
                            source.Duree,
                            source.Type,
                            source.Saison,
                            source.DeconseilleAuxEnfants,
                            source.SpectacleEnfant1Id,
                            source.SpectacleEnfant2Id,
                            source.SpectacleEnfant3Id
                        );
                    SELECT SCOPE_IDENTITY();",
                    connection);

                string? line;
                int lineNumber = 0;
                var errors = new List<string>();

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    try
                    {
                        var values = line.Split(';');
                        if (values.Length < 7)
                        {
                            _logger.LogWarning($"Ligne {lineNumber} ignorée: nombre de colonnes incorrect");
                            continue;
                        }

                        var saison = values[0]?.Trim() ?? throw new ArgumentException("La saison est obligatoire");
                        var titre = values[1]?.Trim() ?? throw new ArgumentException("Le titre est obligatoire");
                        var description = values[2]?.Trim() ?? throw new ArgumentException("La description est obligatoire");
                        var type = values[6]?.Trim() ?? throw new ArgumentException("Le type est obligatoire");

                        if (!int.TryParse(values[7]?.Trim(), out int dureeMinutes) || dureeMinutes <= 0)
                        {
                            throw new ArgumentException("La durée doit être un nombre positif");
                        }

                        // Validation des champs obligatoires
                        ValidateRequiredFields(titre, description, type, dureeMinutes.ToString(), saison);

                        // Conversion des valeurs
                        var titreTronque = TruncateTitre(titre);
                        var dureeTimeSpan = TimeSpan.FromMinutes(dureeMinutes);

                        // Préparation des paramètres
                        insertCommand.Parameters.Clear();
                        insertCommand.Parameters.AddWithValue("@Titre", titreTronque);
                        insertCommand.Parameters.AddWithValue("@Description", description);
                        insertCommand.Parameters.AddWithValue("@Duree", dureeTimeSpan);
                        insertCommand.Parameters.AddWithValue("@Type", type);
                        insertCommand.Parameters.AddWithValue("@Saison", saison);
                        insertCommand.Parameters.AddWithValue("@DeconseilleAuxEnfants", false);
                        insertCommand.Parameters.AddWithValue("@SpectacleEnfant1Id", DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@SpectacleEnfant2Id", DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@SpectacleEnfant3Id", DBNull.Value);

                        // Exécution de la commande
                        var spectacleId = await insertCommand.ExecuteScalarAsync();
                        _logger.LogInformation($"Spectacle traité avec succès: {titreTronque}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Erreur lors de l'importation de la ligne {lineNumber}: {line}");
                        errors.Add($"Erreur à la ligne {lineNumber}: {ex.Message}");
                    }
                }

                if (errors.Any())
                {
                    throw new Exception($"Import terminé avec {errors.Count} erreurs: {string.Join(", ", errors.Take(5))}...");
                }

                _logger.LogInformation("Importation terminée avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'importation depuis le CSV");
                throw;
            }
        }
    }
} 