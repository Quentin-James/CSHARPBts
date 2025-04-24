using DAL.Modeles;

namespace Services.Interfaces
{
    public interface IProgrammationService
    {
        Task<Programmation?> GetByIdAsync(int id);
    }
} 