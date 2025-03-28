using DAL.Interfaces;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
//using Services.CsvImport;

namespace Services.Injection
{
    public static class ServiceInjection
    {
        /// <summary>
        /// Configure le contexte de base de données pour une application WPF.
        /// </summary>
        /// <param name="services">Collection de services.</param>
        /// <param name="config">Configuration de l'application.</param>
        /// <returns>Collection de services mise à jour.</returns>
        public static IServiceCollection ConfigureWpfDbContext(this IServiceCollection services, IConfiguration config)
        {
            // Configurer le contexte SQL Server
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));

            return services;
        }

        /// <summary>
        /// Ajoute les repositories nécessaires pour l'application WPF.
        /// </summary>
        /// <param name="services">Collection de services.</param>
        /// <param name="config">Configuration de l'application.</param>
        /// <returns>Collection de services mise à jour.</returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
        {
            DataAccess.Extensions.Injection.AddRepositories(services, config);

            return services;
        }

        /// <summary>
        /// Configure les services métier pour l'application WPF.
        /// </summary>
        /// <param name="services">Collection de services.</param>
        /// <param name="config">Configuration de l'application.</param>
        /// <returns>Collection de services mise à jour.</returns>
        public static IServiceCollection AddServiceLayer(this IServiceCollection services, IConfiguration config)
        {
            // Ajouter les services spécifiques à l'application WPF
            /* services.AddScoped<ImportService, ImportService>();
            services.AddScoped<ICsvImportService, CsvImportService>();
            */
            return services;
        }
    }
}
