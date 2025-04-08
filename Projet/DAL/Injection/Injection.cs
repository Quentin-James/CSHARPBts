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
            // Ne garder que les repositories essentiels pour l'API
            services.AddScoped<IRepository<Spectacle>, RepoSpectacles>();
            services.AddScoped<IRepository<Programmation>, RepoProgrammation>();
            services.AddScoped<IRepository<Billet>, RepoBillets>();

            // Register generic repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
    }
}

