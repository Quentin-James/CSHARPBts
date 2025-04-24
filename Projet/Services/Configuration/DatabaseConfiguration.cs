using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DAL.Modeles;
using DAL.Interfaces;

namespace Services.Configuration
{
    public static class DatabaseConfiguration
    {
        public static void ConfigureDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            
            services.AddScoped<AppDbContext>();
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        }
    }
} 