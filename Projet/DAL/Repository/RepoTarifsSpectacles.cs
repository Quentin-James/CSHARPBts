using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    internal class RepoTarifsSpectacles : Repository<TarifsSpectacle>, IRepository<TarifsSpectacle>
    {
        public RepoTarifsSpectacles(AppDbContext context) : base(context)
        {
        }
    }
}

