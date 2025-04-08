using Microsoft.Win32;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;

namespace CSVParsing
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7132/") };
        }

        private async void ImportCsvButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Sélectionnez un fichier CSV"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var validationErrors = ValidateCsvFile(filePath);

                if (validationErrors.Count > 0)
                {
                    MessageBox.Show(string.Join("\n", validationErrors), "Erreurs de validation", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    var result = await ImportCsvAsync(filePath);

                    if (result == null)
                    {
                        MessageBox.Show("Une erreur s'est produite lors de l'importation du fichier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (result.HasErrors)
                    {
                        MessageBox.Show(string.Join("\n", result.Errors), "Erreurs d'importation", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("Importation réussie.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        // Lire et valider le CSV avant envoi
        private List<string> ValidateCsvFile(string filePath)
        {
            var errors = new List<string>();
            var expectedColumnCount = 9; // Remplacez par le nombre de colonnes attendu

            using (var reader = new StreamReader(filePath))
            {
                string line;
                int lineNumber = 0;

                // Lire l'en-tête et vérifier les colonnes
                if ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    var headerColumns = line.Split(';');
                    if (headerColumns.Length != expectedColumnCount)
                    {
                        errors.Add($"Ligne {lineNumber} : Nombre de colonnes incorrect dans l'en-tête (attendu : {expectedColumnCount}, trouvé : {headerColumns.Length})");
                        return errors;
                    }
                }

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    var columns = line.Split(';'); // Assurez-vous que le séparateur correspond à votre fichier CSV

                    if (columns.Length != expectedColumnCount)
                    {
                        errors.Add($"Ligne {lineNumber} : Nombre de colonnes incorrect (attendu : {expectedColumnCount}, trouvé : {columns.Length})");
                        continue;
                    }

                    if (!DateTime.TryParse(columns[5], out _))
                    {
                        errors.Add($"Ligne {lineNumber} : La colonne 6 (horaire) doit être une date valide.");
                    }

                    // La colonne 7 (lieu) est une chaîne de caractères, pas de validation spécifique nécessaire

                    if (!decimal.TryParse(columns[7], out _))
                    {
                        errors.Add($"Ligne {lineNumber} : La colonne 8 (prix) doit être un nombre décimal valide.");
                    }
                }
            }

            return errors;
        }

        private async Task<ImportResultDto> ImportCsvAsync(string filePath)
        {
            using (var content = new MultipartFormDataContent())
            {
                var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                content.Add(fileContent, "file", Path.GetFileName(filePath));

                try
                {
                    var requestUri = new Uri(_httpClient.BaseAddress, "api/import/import");
                    MessageBox.Show($"Requête vers l'URL : {requestUri}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    var response = await _httpClient.PostAsync(requestUri, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erreur de l'API : {response.StatusCode} - {errorContent}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }

                    var result = await response.Content.ReadAsAsync<ImportResultDto>();
                    return result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Exception lors de l'appel à l'API : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }
    }
}
