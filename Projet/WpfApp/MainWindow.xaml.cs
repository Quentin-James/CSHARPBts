using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string apiUrl = "https://localhost:7132/api/checkticket"; // Assurez-vous que l'URL pointe vers l'API correcte
            bool isTicketValid = await CheckTicket(apiUrl);
            if (isTicketValid)
            {
                MessageBox.Show("Ticket is valid");
            }
            else
            {
                MessageBox.Show("Ticket is invalid");
            }
        }

        private async Task<bool> CheckTicket(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse>(jsonResponse);
                    return result.IsTicketValid;
                }
                else
                {
                    return false;
                }
            }
        }

        private class ApiResponse
        {
            public bool IsTicketValid { get; set; }
        }
    }
}
