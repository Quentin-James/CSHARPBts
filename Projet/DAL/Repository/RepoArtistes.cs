using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    internal class RepoArtistes : Repository<Artiste>, IRepository<Artiste>
    {
        public RepoArtistes(AppDbContext context) : base(context)
        {
        }
    }
}

