using DAL.Interfaces;
using DAL.Modeles;
using DAL.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.Repository;

namespace DataAccess.Extensions
{
    public static class Injection
    {
        public static IServiceCollection AppDbContext(this IServiceCollection services, IdentityBuilder builder)
        {
            services.AddScoped<IAppDbContext>(service => (IAppDbContext)service.GetRequiredService<AppDbContext>());
            builder.AddEntityFrameworkStores<AppDbContext>();
            return services;
        }

        public static void ConfigureSqlServerContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));
            services.AddIdentityCore<IdentityUser>()
                .AddEntityFrameworkStores<AppDbContext>();
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IRepository<Artiste>, RepoArtistes>();
            services.AddScoped<IRepository<Spectacle>, RepoSpectacles>();
            services.AddScoped<IRepository<TarifsSpectacle>, RepoTarifsSpectacles>();
            services.AddScoped<IRepository<TypesTarif>, RepoTypesTarifs>();
            services.AddScoped<IRepository<Billet>, RepoBillets>();
            services.AddScoped<IRepository<TarifsGroupe>, RepoTarifsGroupes>();
            services.AddScoped<IRepository<ArtisestSpectacles>, RepoArtistesSpectacles>();
            services.AddScoped<IRepository<GroupesSpectacle>, RepoGroupesSpectacles>();
            services.AddScoped<IRepository<Programmation>, RepoProgrammation>();
            services.AddScoped<IRepository<GroupesSpectaclesOrganisation>, RepoGroupesSpectaclesOrganisation>();

            // Register generic repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Ajout de cette ligne

            return services;
        }
    }
}

