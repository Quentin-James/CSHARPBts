using Microsoft.Win32;
using Services.DTOs;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;

namespace CSVParsing
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private bool _saisonImported = false;
        private const int COLONNES_SAISON = 17;
        private const int COLONNES_BILLET = 9;

        public MainWindow()
        {
            InitializeComponent();
            
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost:8080/")
            };
            
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            // button always enabled
            ImportButton.IsEnabled = true;
            // enabled when saison is imported
            ChevauchementButton.IsEnabled = _saisonImported;
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
                try
                {
                    ImportButton.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Visible;
                    ProgressBar.Value = 0;

                    var firstLine = File.ReadLines(openFileDialog.FileName).First();
                    var columnCount = firstLine.Split(';').Length;

                    
                    if (columnCount == COLONNES_SAISON && !_saisonImported)
                    {
                        await ImportCsvAsync(openFileDialog.FileName);
                        _saisonImported = true;
                        UpdateButtonsState();
                        MessageBox.Show("Import des saisons réussi. Vous pouvez maintenant importer les billets.", 
                                      "Succès", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Information);
                    }
                    else if (columnCount == COLONNES_BILLET && !_saisonImported)
                    {
                        MessageBox.Show("Vous devez d'abord importer le fichier CSV des saisons.", 
                                      "Import requis", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Warning);
                    }
                    else if (columnCount != COLONNES_SAISON && columnCount != COLONNES_BILLET)
                    {
                        MessageBox.Show($"Format de fichier incorrect. Le fichier doit avoir {COLONNES_SAISON} colonnes pour les saisons ou {COLONNES_BILLET} colonnes pour les billets.", 
                                      "Erreur", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Error);
                    }
                    else
                    {
                        await ImportCsvAsync(openFileDialog.FileName);
                        MessageBox.Show("Importation des billets réussie.", 
                                      "Succès", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    ImportButton.IsEnabled = true;
                    ProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ChevauchementButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_saisonImported)
            {
                MessageBox.Show("Vous devez d'abord importer le fichier CSV des saisons.", 
                              "Import requis", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            var chevauchementWindow = new ChevauchementWindow();
            chevauchementWindow.Owner = this;
            chevauchementWindow.ShowDialog();
        }

        private async Task ImportCsvAsync(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            var totalLines = lines.Length - 1; 
            var processedLines = 0;

            var firstLine = lines.First();
            var columnCount = firstLine.Split(';').Length;
            var endpoint = columnCount == COLONNES_SAISON ? "spectacles" : "billets";

            using var form = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            using var streamContent = new StreamContent(fileStream);
            
            form.Add(streamContent, "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync($"api/csv-import/{endpoint}", form);
            response.EnsureSuccessStatusCode();

            
            while (processedLines < totalLines)
            {
                processedLines++;
                ProgressBar.Value = (double)processedLines / totalLines * 100;
                await Task.Delay(10); 
            }
        }
    }
}
