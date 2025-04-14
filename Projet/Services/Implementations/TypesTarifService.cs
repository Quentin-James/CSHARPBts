using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Modeles;
using Services.Interfaces;

namespace Services.Implementations
{
    public class TypesTarifService : ITypesTarifService
    {
        private readonly IRepository<TypesTarif> _typesTarifRepository;

        public TypesTarifService(IRepository<TypesTarif> typesTarifRepository)
        {
            _typesTarifRepository = typesTarifRepository;
        }

        public async Task<TypesTarif?> GetByIdAsync(int id)
        {
            return await _typesTarifRepository.GetByIdAsync(id);
        }
    }
} 