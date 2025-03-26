using DAL.Modeles;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Artiste> Artistes { get; set; }
        DbSet<Billet> Billets { get; set; }
        DbSet<GroupesSpectacle> GroupesSpectacles { get; set; }
        DbSet<GroupesSpectaclesOrganisation> GroupesSpectaclesOrganisations { get; set; }
        DbSet<Programmation> Programmations { get; set; }
        DbSet<Spectacle> Spectacles { get; set; }
        DbSet<TarifsGroupe> TarifsGroupes { get; set; }
        DbSet<TarifsSpectacle> TarifsSpectacles { get; set; }
        DbSet<TypesTarif> TypesTarifs { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
