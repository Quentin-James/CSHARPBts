using DAL.Modeles;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Billet> Billets { get; set; }
        DbSet<Programmation> Programmations { get; set; }
        DbSet<Spectacle> Spectacles { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
