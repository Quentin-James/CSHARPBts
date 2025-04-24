namespace Services.DTOs
{
    public class ChevauchementDto
    {
        public int SpectacleId { get; set; }
        public string Titre { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? HeureDebut { get; set; }
        public TimeOnly? HeureFin { get; set; }
        public int SpectacleChevaucheId { get; set; }
        public string TitreChevauchement { get; set; }
        public TimeOnly? HeureDebutChevauchement { get; set; }
        public TimeOnly? HeureFinChevauchement { get; set; }
    }
} 