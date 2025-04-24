using DAL.Interfaces;
using DAL.Modeles;

namespace DAL.Repository
{
    public class RepoBillets : Repository<Billet>, IRepository<Billet>
    {
        public RepoBillets(AppDbContext context) : base(context)
        {
        }
    }
}

