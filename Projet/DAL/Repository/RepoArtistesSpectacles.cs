using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    internal class RepoArtistesSpectacles : Repository<ArtisestSpectacles>, IRepository<ArtisestSpectacles>
    {
        public RepoArtistesSpectacles(AppDbContext context) : base(context)
        {
        }
    }
}