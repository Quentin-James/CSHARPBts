using DAL.Interfaces;
using DAL.Modeles;

namespace DAL.Repository
{
    public class RepoGroupesSpectacle : Repository<GroupesSpectacle>, IRepository<GroupesSpectacle>
    {
        public RepoGroupesSpectacle(AppDbContext context) : base(context)
        {
        }
    }
} 