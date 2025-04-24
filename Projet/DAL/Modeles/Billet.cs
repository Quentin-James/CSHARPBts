namespace DAL.Modeles;

public partial class Billet
{
    public int BilletId { get; set; }

    public string? Civilite { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public decimal PrixAchat { get; set; }

    public int? TarifId { get; set; }

    public int? ProgrammationId { get; set; }

    public virtual Programmation? Programmation { get; set; }

    public virtual TypesTarif? Tarif { get; set; }
}
