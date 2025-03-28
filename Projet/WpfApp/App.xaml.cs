using DataAccess.Extensions; // Pour l'injection de la DAL
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.CsvImport;
using Services.Injection; // Pour l'injection de la couche Service
using System;
using System.Windows;
using WpfApp;

namespace WPF
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        private IHost _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Construire l'hôte pour l'injection de dépendance
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Configurer la DAL
                    services.ConfigureSqlServerContext(context.Configuration);
                    DataAccess.Extensions.Injection.AddRepositories(services, context.Configuration);

                    // Configurer les services métier
                    Services.Injection.ServiceInjection.AddServiceLayer(services, context.Configuration);

                    // Ajouter les vues WPF
                    services.AddTransient<MainWindow>();

                    services.AddScoped<ICsvImportService, CsvImportService>();
                })
                .Build();

            ServiceProvider = _host.Services;

            // Lancer la fenêtre principale
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();
            base.OnExit(e);
        }
    }
}

