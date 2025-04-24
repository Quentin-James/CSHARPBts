using Services.DTOs;

namespace Services.Interfaces
{
    public interface IChevauchementService
    {
        Task<List<string>> GetTableNamesAsync();
        Task<List<ChevauchementDto>> GetChevauchementsAsync();
        Task InsertTestDataAsync();
    }
} 