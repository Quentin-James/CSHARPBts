namespace Services.DTOs
{
    public class SpectacleDto
    {
        public int SpectacleId { get; set; }
        public string Titre { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public TimeOnly? Duree { get; set; }
    }
}
