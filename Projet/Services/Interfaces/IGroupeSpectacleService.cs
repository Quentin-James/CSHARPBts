using DAL.Modeles;

namespace Services.Interfaces
{
    public interface IGroupeSpectacleService
    {
        Task<GroupesSpectacle?> GetByIdAsync(int id);
    }
} 