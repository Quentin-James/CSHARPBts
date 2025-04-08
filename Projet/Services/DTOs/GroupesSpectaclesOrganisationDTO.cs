namespace Services.DTOs
{
    public class GroupesSpectaclesOrganisationDto
    {
        public int GroupeId { get; set; }
        public string? TypeSpectacle { get; set; }
        public TimeOnly? Duree { get; set; }
        public string? Description { get; set; }
    }
}
