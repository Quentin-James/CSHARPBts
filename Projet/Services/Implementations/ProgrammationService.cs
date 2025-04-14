using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Modeles;
using Services.Interfaces;

namespace Services.Implementations
{
    public class ProgrammationService : IProgrammationService
    {
        private readonly IRepository<Programmation> _programmationRepository;

        public ProgrammationService(IRepository<Programmation> programmationRepository)
        {
            _programmationRepository = programmationRepository;
        }

        public async Task<Programmation?> GetByIdAsync(int id)
        {
            return await _programmationRepository.GetByIdAsync(id);
        }
    }
} 