using DAL.Interfaces;
using DAL.Modeles;
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
        public static IServiceCollection AddDbContextMini(this IServiceCollection services, IdentityBuilder builder)
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

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Artiste>, Repository<Artiste>>();
            services.AddScoped<IRepository<Spectacle>, Repository<Spectacle>>();
            services.AddScoped<IRepository<TarifsSpectacle>, Repository<TarifsSpectacle>>();
            services.AddScoped<IRepository<TypesTarif>, Repository<TypesTarif>>();
            services.AddScoped<IRepository<Billet>, Repository<Billet>>();
            services.AddScoped<IRepository<TarifsGroupe>, Repository<TarifsGroupe>>();
            services.AddScoped<IRepository<ArtisestSpectacles>, Repository<ArtisestSpectacles>>();
            services.AddScoped<IRepository<GroupesSpectacle>, Repository<GroupesSpectacle>>();
            services.AddScoped<IRepository<Programmation>, Repository<Programmation>>();
            services.AddScoped<IRepository<GroupesSpectaclesOrganisation>, Repository<GroupesSpectaclesOrganisation>>();

            return services;
        }
    }
    
}
