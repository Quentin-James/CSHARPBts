using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    internal class RepoProgrammation : Repository<Programmation>, IRepository<Programmation>
    {
        public RepoProgrammation(AppDbContext context) : base(context)
        {
        }
    }
}
