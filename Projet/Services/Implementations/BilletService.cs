using DAL.Interfaces;
using DAL.Modeles;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace Services.Implementations
{
    public class BilletService : IBilletService
    {
        private readonly IRepository<Billet> _billetRepository;
        private readonly AppDbContext _context;

        public BilletService(IRepository<Billet> billetRepository, AppDbContext context)
        {
            _billetRepository = billetRepository;
            _context = context;
        }

        public async Task<Billet?> GetByIdAsync(int id)
        {
            return await _context.Billets
                .Include(b => b.Programmation)
                .ThenInclude(p => p.Spectacle)
                .FirstOrDefaultAsync(b => b.BilletId == id);
        }
    }
} 