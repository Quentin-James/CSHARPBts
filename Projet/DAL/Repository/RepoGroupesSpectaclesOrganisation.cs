using DAL.Interfaces;
using Models.Repository;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    internal class RepoGroupesSpectaclesOrganisation : Repository<GroupesSpectaclesOrganisation>, IRepository<GroupesSpectaclesOrganisation>
    {
        public RepoGroupesSpectaclesOrganisation(AppDbContext context) : base(context)
        {
        }
    }
}

