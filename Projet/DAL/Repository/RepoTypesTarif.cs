using DAL.Interfaces;
using DAL.Modeles;

namespace DAL.Repository
{
    public class RepoTypesTarif : Repository<TypesTarif>, IRepository<TypesTarif>
    {
        public RepoTypesTarif(AppDbContext context) : base(context)
        {
        }
    }
} 