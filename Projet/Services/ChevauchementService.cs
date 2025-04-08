using DAL.Modeles;
using Microsoft.EntityFrameworkCore;
using Services.DTOs;

namespace Services
{
    public class ChevauchementService
    {
        private readonly AppDbContext _context;

        public ChevauchementService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SpectacleChevauchementDto>> DetecterChevauchements()
        {
            var sql = @"
                SELECT 
                    p1.SpectacleID as SpectacleId, 
                    p1.Heure as HeureDebut, 
                    DATEADD(MINUTE, DATEDIFF(MINUTE, '00:00', s.Duree), p1.Heure) as HeureFin,
                    s.Nom as NomSpectacle
                FROM 
                    Programmation p1
                JOIN 
                    Spectacles s ON p1.SpectacleID = s.SpectacleID
                WHERE 
                    EXISTS (
                        SELECT 1
                        FROM Programmation p2
                        WHERE 
                            p1.SpectacleID != p2.SpectacleID   
                            AND p1.Date = p2.Date            
                            AND (
                                (p1.Heure >= p2.Heure AND p1.Heure < DATEADD(MINUTE, DATEDIFF(MINUTE, '00:00', s.Duree), p2.Heure))
                                OR 
                                (p2.Heure >= p1.Heure AND p2.Heure < DATEADD(MINUTE, DATEDIFF(MINUTE, '00:00', s.Duree), p1.Heure))
                            )
                    )";

            var chevauchements = await _context.Database
                .SqlQueryRaw<SpectacleChevauchementDto>(sql)
                .ToListAsync();

            return chevauchements;
        }
    }
} 