using System.Threading.Tasks;
using DAL.Modeles;

namespace Services.Interfaces
{
    public interface ITypesTarifService
    {
        Task<TypesTarif?> GetByIdAsync(int id);
    }
} 