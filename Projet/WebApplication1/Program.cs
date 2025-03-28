using DataAccess.Extensions;
using Microsoft.OpenApi.Models;
using Services.Interfaces;
using Services;
using Microsoft.AspNetCore.Identity;
using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ajout des services nécessaires
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger : Ajout de la documentation de l'API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "API pour la gestion des spectacles et des billets",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@example.com"
        }
    });
});


var app = builder.Build();

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;  // Cette ligne permettra d'accéder directement à Swagger à la racine (localhost:port/).
});

// Middleware pour rediriger les requêtes HTTP vers HTTPS
app.UseHttpsRedirection();

// Middleware pour gérer l'autorisation (si vous utilisez l'authentification)
app.UseAuthorization();

// Mapping des contrôleurs
app.MapControllers();

// Configurer une route par défaut pour servir l'index.html (utile si vous avez un frontend à servir)
app.MapFallbackToFile("index.html");

app.Run();
