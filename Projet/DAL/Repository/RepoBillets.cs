using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    internal class RepoBillets : Repository<Billet>, IRepository<Billet>
    {
        public RepoBillets(AppDbContext context) : base(context)
        {
        }
    }
}

