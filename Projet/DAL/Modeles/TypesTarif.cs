namespace DAL.Modeles;

public partial class TypesTarif
{
    public int TarifId { get; set; }

    public string NomTarif { get; set; } = null!;

    public virtual ICollection<Billet> Billets { get; set; } = new List<Billet>();

    public virtual ICollection<TarifsGroupe> TarifsGroupes { get; set; } = new List<TarifsGroupe>();

    public virtual ICollection<TarifsSpectacle> TarifsSpectacles { get; set; } = new List<TarifsSpectacle>();
}
