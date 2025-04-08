namespace Services.DTOs
{
    public class SpectacleChevauchementDto
    {
        public int SpectacleId { get; set; }
        public DateTime HeureDebut { get; set; }
        public DateTime HeureFin { get; set; }
        public string NomSpectacle { get; set; } = string.Empty;
    }
} 