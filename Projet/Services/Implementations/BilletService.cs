using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Modeles;
using Services.Interfaces;

namespace Services.Implementations
{
    public class BilletService : IBilletService
    {
        private readonly IRepository<Billet> _billetRepository;

        public BilletService(IRepository<Billet> billetRepository)
        {
            _billetRepository = billetRepository;
        }

        public async Task<Billet?> GetByIdAsync(int id)
        {
            return await _billetRepository.GetByIdAsync(id);
        }
    }
} 