using System.Windows;
using Services.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace CSVParsing
{
    public partial class ChevauchementWindow : Window
    {
        private readonly HttpClient _httpClient;

        public ChevauchementWindow()
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
            
            LoadChevauchements();
        }

        private async void LoadChevauchements()
        {
            try
            {
                var chevauchements = await _httpClient.GetFromJsonAsync<List<ChevauchementDto>>("api/spectacles/chevauchements");
                ChevauchementGrid.ItemsSource = chevauchements;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Erreur lors de la récupération des chevauchements : {ex.Message}", 
                              "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
    }
} 