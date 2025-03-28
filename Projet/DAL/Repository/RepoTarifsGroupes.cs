using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    internal class RepoTarifsGroupes : Repository<TarifsGroupe>, IRepository<TarifsGroupe>
    {
        public RepoTarifsGroupes(AppDbContext context) : base(context)
        {
        }
    }
}

