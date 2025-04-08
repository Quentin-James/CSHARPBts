namespace Services.DTOs
{
    public class ProgrammationDto
    {
        public int ProgrammationId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Heure { get; set; }
        public string Lieu { get; set; }
        public int SpectacleId { get; set; }
        public DateTime DateHeure => Date.ToDateTime(Heure);
    }
}
