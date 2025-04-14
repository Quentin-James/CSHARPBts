using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;

namespace CSVParsing
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;

        public MainWindow()
        {
            InitializeComponent();
            
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7132/")
            };
            
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

                    await ImportCsvAsync(openFileDialog.FileName);
                    MessageBox.Show("Importation réussie.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Vérification des chevauchements
                    var responseChev = await _httpClient.GetAsync("api/spectacles/chevauchements");
                    if (responseChev.IsSuccessStatusCode)
                    {
                        var chevauchements = await responseChev.Content.ReadFromJsonAsync<string[]>();
                        if (chevauchements != null && chevauchements.Length > 0)
                        {
                            MessageBox.Show(
                                $"Attention, il y a {chevauchements.Length} chevauchement(s) :\n\n" + 
                                string.Join("\n", chevauchements),
                                "Chevauchements détectés",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning
                            );
                        }
                    }

                    // Vérification des billets
                    var responseBillets = await _httpClient.GetAsync("api/billets/verification");
                    if (responseBillets.IsSuccessStatusCode)
                    {
                        var billetsInvalides = await responseBillets.Content.ReadFromJsonAsync<string[]>();
                        if (billetsInvalides != null && billetsInvalides.Length > 0)
                        {
                            MessageBox.Show(
                                $"Attention, il y a {billetsInvalides.Length} billet(s) invalide(s) :\n\n" + 
                                string.Join("\n", billetsInvalides),
                                "Billets invalides détectés",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning
                            );
                        }
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

        private async Task ImportCsvAsync(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            var totalLines = lines.Length - 1; // -1 pour l'en-tête
            var processedLines = 0;

            var firstLine = lines.First();
            var columnCount = firstLine.Split(';').Length;
            var endpoint = columnCount == 9 ? "billets" : "spectacles";

            using var form = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            using var streamContent = new StreamContent(fileStream);
            
            form.Add(streamContent, "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync($"api/csv-import/{endpoint}", form);
            response.EnsureSuccessStatusCode();

            // Mise à jour de la progression
            while (processedLines < totalLines)
            {
                processedLines++;
                ProgressBar.Value = (double)processedLines / totalLines * 100;
                await Task.Delay(10); // Petit délai pour voir la progression
            }
        }
    }
}
