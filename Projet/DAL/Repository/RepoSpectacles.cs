using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    internal class RepoSpectacles : Repository<Spectacle>, IRepository<Spectacle>
    {
        public RepoSpectacles(AppDbContext context) : base(context)
        {
        }
    }
}

