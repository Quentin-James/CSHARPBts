using System.Threading.Tasks;
using DAL.Modeles;

namespace Services.Interfaces
{
    public interface IBilletService
    {
        Task<Billet?> GetByIdAsync(int id);
    }
} 