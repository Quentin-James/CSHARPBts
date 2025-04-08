using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Services.CsvImport;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new Exception("ConnectionString 'DefaultConnection' manquante");
builder.Services.AddDbContext<DAL.Modeles.AppDbContext>(opt => opt.UseSqlServer(connectionString));

// Services
builder.Services.AddScoped<ICsvImportService, CsvImportService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { 
    Title = "API CSV Import", 
    Version = "v1" 
}));

// Logging
builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Information);

// CORS
builder.Services.AddCors(opt => opt.AddPolicy("AllowAll", 
    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Build and configure app
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API CSV Import v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();