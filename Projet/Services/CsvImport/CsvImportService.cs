using DAL.Modeles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace Services.CsvImport
{
    public interface ICsvImportService
    {
        Task ImportSpectaclesAsync(string filePath);
        Task ImportBilletsAsync(string filePath);
    }

    public class CsvImportService : ICsvImportService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CsvImportService> _logger;

        public CsvImportService(AppDbContext dbContext, ILogger<CsvImportService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ImportSpectaclesAsync(string filePath)
        {
            var encoding = GetFileEncoding(filePath);
            using var reader = new StreamReader(filePath, encoding);
            await reader.ReadLineAsync(); 
            
            string? line;
            int lineNumber = 0;
            var errors = new List<string>();

            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineNumber++;
                try
                {
                    var columns = line.Split(';');
                    if (columns.Length < 7) 
                    {
                        errors.Add($"Données invalides à la ligne {lineNumber}: colonnes insuffisantes");
                        continue;
                    }

                    using var transaction = await _dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var titre = Truncate(columns[1].Trim(), 20);
                        var spectacle = await _dbContext.Spectacles
                            .FirstOrDefaultAsync(s => s.Titre == titre);

                        if (spectacle == null)
                        {
                            spectacle = new Spectacle
                            {
                                Titre = titre,
                                Description = columns[2].Trim(),
                                Type = Truncate(columns[6].Trim(), 20),
                                Duree = ParseDuration(columns[7]),
                                Saison = columns[0].Trim()
                            };
                            _dbContext.Spectacles.Add(spectacle);
                            await _dbContext.SaveChangesAsync();

                           
                            for (int i = 3; i <= 5; i++)
                            {
                                if (i < columns.Length && !string.IsNullOrWhiteSpace(columns[i]))
                                    await AddArtiste(spectacle.SpectacleId, Truncate(columns[i].Trim(), 15));
                            }

                            if (columns.Length > 10)
                            {
                                if (decimal.TryParse(columns[8], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tarifPlein))
                                    await AddTarif(spectacle.SpectacleId, "Plein", tarifPlein);
                                
                                if (decimal.TryParse(columns[9], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tarifReduit))
                                    await AddTarif(spectacle.SpectacleId, "Réduit", tarifReduit);
                                
                                if (decimal.TryParse(columns[10], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tarifEnfant))
                                    await AddTarif(spectacle.SpectacleId, "Enfant", tarifEnfant);
                            }
                        }

                        if (columns.Length > 12 && !string.IsNullOrWhiteSpace(columns[11]) && !string.IsNullOrWhiteSpace(columns[12]))
                        {
                            if (DateTime.TryParse(columns[11].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateHeure))
                            {
                                await AddProgrammation(spectacle.SpectacleId, dateHeure, Truncate(columns[12].Trim(), 20));
                            }
                        }

                        if (columns.Length > 15)
                        {
                            if (!string.IsNullOrWhiteSpace(columns[13]))
                            {
                                var enfant1 = await GetOrCreateSpectacle(columns[13].Trim());
                                spectacle.SpectacleEnfant1Id = enfant1.SpectacleId;
                            }

                            if (!string.IsNullOrWhiteSpace(columns[14]))
                            {
                                var enfant2 = await GetOrCreateSpectacle(columns[14].Trim());
                                spectacle.SpectacleEnfant2Id = enfant2.SpectacleId;
                            }

                            if (!string.IsNullOrWhiteSpace(columns[15]))
                            {
                                var enfant3 = await GetOrCreateSpectacle(columns[15].Trim());
                                spectacle.SpectacleEnfant3Id = enfant3.SpectacleId;
                            }

                            if (columns.Length > 16)
                            {
                                spectacle.DeconseilleAuxEnfants = columns[16].Trim().ToLower() == "true";
                            }
                        }

                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur à la ligne {LineNumber}", lineNumber);
                    errors.Add($"Erreur à la ligne {lineNumber}: {ex.Message}");
                }
            }

            if (errors.Any())
                throw new Exception($"Import terminé avec {errors.Count} erreurs: {string.Join(", ", errors.Take(5))}...");
        }

        private async Task AddArtiste(int spectacleId, string nom)
        {
            var artiste = await _dbContext.Artistes.FirstOrDefaultAsync(a => a.Nom == nom) 
                ?? new Artiste { Nom = nom };
                
            if (artiste.ArtisteId == 0)
            {
                _dbContext.Artistes.Add(artiste);
                await _dbContext.SaveChangesAsync();
            }
            
            var spectacle = await _dbContext.Spectacles
                .Include(s => s.Artistes)
                .FirstOrDefaultAsync(s => s.SpectacleId == spectacleId);
                
            if (spectacle != null && !spectacle.Artistes.Any(a => a.ArtisteId == artiste.ArtisteId))
            {
                spectacle.Artistes.Add(artiste);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task AddTarif(int spectacleId, string nomTarif, decimal prix)
        {
            var tarif = await _dbContext.TypesTarifs.FirstOrDefaultAsync(t => t.NomTarif == nomTarif)
                ?? new TypesTarif { NomTarif = nomTarif };
                
            if (tarif.TarifId == 0)
            {
                _dbContext.TypesTarifs.Add(tarif);
                await _dbContext.SaveChangesAsync();
            }
            
            var tarifSpectacle = new TarifsSpectacle
            {
                SpectacleId = spectacleId,
                TarifId = tarif.TarifId,
                Prix = prix
            };
            
            _dbContext.TarifsSpectacles.Add(tarifSpectacle);
            await _dbContext.SaveChangesAsync();
        }

        private async Task AddProgrammation(int spectacleId, DateTime dateHeure, string lieu)
        {
            var dateOnly = DateOnly.FromDateTime(dateHeure.Date);
            var timeOnly = TimeOnly.FromTimeSpan(dateHeure.TimeOfDay);
            
            var programmationExists = await _dbContext.Programmations
                .AnyAsync(p => 
                    p.SpectacleId == spectacleId &&
                    p.Date == dateOnly &&
                    p.Lieu == lieu);
                    
            if (!programmationExists)
            {
                var programmation = new Programmation
                {
                    SpectacleId = spectacleId,
                    Date = dateOnly,
                    Heure = timeOnly,
                    Lieu = lieu
                };
                _dbContext.Programmations.Add(programmation);
                await _dbContext.SaveChangesAsync();
            }
        }

        private TimeOnly? ParseDuration(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value.Trim(), out int minutes))
                return null;
                
            return new TimeOnly(minutes / 60, minutes % 60);
        }

        public async Task ImportBilletsAsync(string filePath)
        {
            var encoding = GetFileEncoding(filePath);
            using var reader = new StreamReader(filePath, encoding);
            await reader.ReadLineAsync(); // Skip header
            
            string? line;
            int lineNumber = 0;
            var errors = new List<string>();

            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineNumber++;
                try
                {
                    var columns = line.Split(';');
                    if (columns.Length < 9)
                    {
                        errors.Add($"Données invalides à la ligne {lineNumber}: colonnes insuffisantes");
                        continue;
                    }

                    var civilite = Truncate(columns[1].Trim(), 10);
                    var nom = Truncate(columns[2].Trim(), 12);
                    var prenom = Truncate(columns[3].Trim(), 12);
                    var spectacleNom = Truncate(columns[4].Trim(), 20);
                    
                    if (!DateTime.TryParse(columns[5].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime horaire))
                    {
                        errors.Add($"Format de date invalide à la ligne {lineNumber}: '{columns[5].Trim()}'");
                        continue;
                    }
                    
                    var lieu = Truncate(columns[6].Trim(), 20);
                    
                    if (!decimal.TryParse(columns[7].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prix))
                    {
                        errors.Add($"Format de prix invalide à la ligne {lineNumber}: '{columns[7].Trim()}'");
                        continue;
                    }
                    
                    var typeTarifNom = Truncate(columns[8].Trim(), 20);

                    using var transaction = await _dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var spectacle = await _dbContext.Spectacles
                            .FirstOrDefaultAsync(s => s.Titre == spectacleNom);

                        if (spectacle == null)
                        {
                            errors.Add($"Spectacle non trouvé à la ligne {lineNumber}: '{spectacleNom}'");
                            continue;
                        }

                        var dateOnly = DateOnly.FromDateTime(horaire.Date);
                        var timeOnly = TimeOnly.FromTimeSpan(horaire.TimeOfDay);
                        
                        var programmation = await _dbContext.Programmations
                            .FirstOrDefaultAsync(p => 
                                p.SpectacleId == spectacle.SpectacleId && 
                                p.Date == dateOnly &&
                                p.Lieu == lieu);

                        if (programmation == null)
                        {
                            errors.Add($"Programmation non trouvée à la ligne {lineNumber}: '{spectacleNom}' le {dateOnly} à {timeOnly}");
                            continue;
                        }

                        var tarif = await _dbContext.TypesTarifs
                            .FirstOrDefaultAsync(t => t.NomTarif == typeTarifNom);

                        if (tarif == null)
                        {
                            _logger.LogWarning($"Type de tarif non trouvé: '{typeTarifNom}'. Création d'un nouveau type de tarif.");
                            tarif = new TypesTarif { NomTarif = typeTarifNom };
                            _dbContext.TypesTarifs.Add(tarif);
                            await _dbContext.SaveChangesAsync();
                        }

                        var billetExistant = await _dbContext.Billets
                            .FirstOrDefaultAsync(b => 
                                b.ProgrammationId == programmation.ProgrammationId &&
                                b.Nom == nom &&
                                b.Prenom == prenom &&
                                b.TarifId == tarif.TarifId);

                        if (billetExistant != null)
                        {
                            _logger.LogWarning($"Billet déjà existant à la ligne {lineNumber} pour {nom} {prenom}");
                            continue;
                        }

                        var billet = new Billet
                        {
                            Civilite = civilite,
                            Nom = nom,
                            Prenom = prenom,
                            PrixAchat = prix,
                            TarifId = tarif.TarifId,
                            ProgrammationId = programmation.ProgrammationId
                        };

                        _dbContext.Billets.Add(billet);
                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Erreur lors de l'import du billet à la ligne {LineNumber}", lineNumber);
                        errors.Add($"Erreur à la ligne {lineNumber}: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur à la ligne {LineNumber}", lineNumber);
                    errors.Add($"Erreur à la ligne {lineNumber}: {ex.Message}");
                }
            }
            
            if (errors.Any())
                throw new Exception($"Import terminé avec {errors.Count} erreurs: {string.Join(", ", errors.Take(5))}...");
        }

        private async Task<Spectacle> GetOrCreateSpectacle(string nom)
        {
            var spectacle = await _dbContext.Spectacles.FirstOrDefaultAsync(s => s.Titre == nom);
            
            if (spectacle == null)
            {
                spectacle = new Spectacle
                {
                    Titre = Truncate(nom, 20),
                    Description = $"Importé depuis CSV - {nom}",
                    Type = "Standard"
                };
                _dbContext.Spectacles.Add(spectacle);
                await _dbContext.SaveChangesAsync();
            }
            
            return spectacle;
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
        
        private static string Truncate(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
