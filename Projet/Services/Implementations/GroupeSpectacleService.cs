using DAL.Interfaces;
using DAL.Modeles;
using Services.Interfaces;

namespace Services.Implementations
{
    public class GroupeSpectacleService : IGroupeSpectacleService
    {
        private readonly IRepository<GroupesSpectacle> _groupeSpectacleRepository;

        public GroupeSpectacleService(IRepository<GroupesSpectacle> groupeSpectacleRepository)
        {
            _groupeSpectacleRepository = groupeSpectacleRepository;
        }

        public async Task<GroupesSpectacle?> GetByIdAsync(int id)
        {
            return await _groupeSpectacleRepository.GetByIdAsync(id);
        }
    }
} 