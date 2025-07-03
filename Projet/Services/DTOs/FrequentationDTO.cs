namespace Services.DTOs
{
    public class FrequentationDTO
    {
        public int SpectacleId { get; set; }
        public string TitreSpectacle { get; set; } = string.Empty;
        public DateOnly DateRepresentation { get; set; }
        public int NombreBilletsVendus { get; set; }
    }
} 