using DAL.Modeles;
using Services.DTOs;

namespace Services.Interfaces
{
    public interface IBilletService
    {
        Task<Billet?> GetByIdAsync(int id);
        Task<IEnumerable<FrequentationDTO>> GetFrequentationBySpectacleAsync(int spectacleId);
    }
} 