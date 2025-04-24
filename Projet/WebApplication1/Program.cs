using DataAccess.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Services.Configuration;
using Services.CsvImport;
using Services.Implementations;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuration de Kestrel pour écouter sur toutes les interfaces
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new Exception("ConnectionString 'DefaultConnection' manquante");
builder.Services.ConfigureDatabase(connectionString);

// Repositories
builder.Services.AddRepositories(builder.Configuration);

// Services
builder.Services.AddScoped<ICsvImportService, CsvImportService>();
builder.Services.AddScoped<IBilletService, BilletService>();
builder.Services.AddScoped<IProgrammationService, ProgrammationService>();
builder.Services.AddScoped<IGroupeSpectacleService, GroupeSpectacleService>();
builder.Services.AddScoped<ISpectacleService, SpectacleService>();
builder.Services.AddScoped<ITypesTarifService, TypesTarifService>();
builder.Services.AddScoped<Services.Interfaces.IChevauchementService, Services.ChevauchementService>();
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

// app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();