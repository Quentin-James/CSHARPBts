using DAL.Interfaces;
using DAL.Modeles;

namespace DAL.Repository
{
    internal class RepoProgrammation : Repository<Programmation>, IRepository<Programmation>
    {
        public RepoProgrammation(AppDbContext context) : base(context)
        {
        }
    }
}
