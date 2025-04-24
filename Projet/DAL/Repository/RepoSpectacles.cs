using DAL.Interfaces;
using DAL.Modeles;

namespace DAL.Repository
{
    internal class RepoSpectacles : Repository<Spectacle>, IRepository<Spectacle>
    {
        public RepoSpectacles(AppDbContext context) : base(context)
        {
        }
    }
}

