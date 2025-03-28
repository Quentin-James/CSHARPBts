using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    internal class RepoGroupesSpectacles : Repository<GroupesSpectacle>, IRepository<GroupesSpectacle>
    {
        public RepoGroupesSpectacles(AppDbContext context) : base(context)
        {
        }
    }
}

