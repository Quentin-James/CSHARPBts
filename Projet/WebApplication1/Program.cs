using DataAccess.Extensions;
using Microsoft.OpenApi.Models;
using Services;
using Microsoft.AspNetCore.Identity;
using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;
using System;
using Services.CsvImport;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new Exception("ConnectionString 'DefaultConnection' manquante");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Services
builder.Services.AddScoped<ICsvImportService, CsvImportService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
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

// Logging
builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Information);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Build and configure app
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

// Configurer une route par défaut pour servir l'index.html
app.MapFallbackToFile("index.html");

app.Run();