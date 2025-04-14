namespace Services.DTOs
{
    public class BilletDTO
    {
        public int BilletId { get; set; }
        public string Civilite { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public decimal PrixAchat { get; set; }
        public int? TarifId { get; set; }
        public int? ProgrammationId { get; set; }
    }
}
