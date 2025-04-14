using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Modeles;
using Services.Interfaces;

namespace Services.Implementations
{
    public class SpectacleService : ISpectacleService
    {
        private readonly IRepository<Spectacle> _spectacleRepository;

        public SpectacleService(IRepository<Spectacle> spectacleRepository)
        {
            _spectacleRepository = spectacleRepository;
        }

        public async Task<Spectacle?> GetByIdAsync(int id)
        {
            return await _spectacleRepository.GetByIdAsync(id);
        }
    }
} 