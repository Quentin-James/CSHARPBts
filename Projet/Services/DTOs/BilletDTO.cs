namespace Services.DTOs
{
    public class BilletDTO
    {
        public string? Civilite { get; set; }
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public decimal PrixAchat { get; set; }
        public int? TarifId { get; set; }
        public int? ProgrammationId { get; set; }
    }
}
