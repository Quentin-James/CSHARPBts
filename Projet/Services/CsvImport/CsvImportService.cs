using System;
using System.IO;
using System.Threading.Tasks;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Services.CsvImport
{
    public interface ICsvImportService
    {
        Task ImportSpectaclesAsync(string filePath);
        Task ImportBilletsAsync(string filePath);
    }

    public class CsvImportService : ICsvImportService
    {
        private readonly AppDbContext _dbContext;

        public CsvImportService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ImportSpectaclesAsync(string filePath)
        {
            // Logic to read and import Spectacles from CSV
            using var reader = new StreamReader(filePath);
            // ...parse CSV and insert into database...
            await _dbContext.SaveChangesAsync();
        }

        public async Task ImportBilletsAsync(string filePath)
        {
            // Logic to read and import Billets from CSV
            using var reader = new StreamReader(filePath);
            // ...parse CSV and insert into database...
            await _dbContext.SaveChangesAsync();
        }
    }
}
