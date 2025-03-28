using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    internal class RepoTypesTarifs : Repository<TypesTarif>, IRepository<TypesTarif>
    {
        public RepoTypesTarifs(AppDbContext context) : base(context)
        {
        }
    }
}

