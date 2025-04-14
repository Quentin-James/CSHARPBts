using System.Threading.Tasks;
using DAL.Modeles;

namespace Services.Interfaces
{
    public interface ISpectacleService
    {
        Task<Spectacle?> GetByIdAsync(int id);
    }
} 