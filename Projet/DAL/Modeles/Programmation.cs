using System;
using System.Collections.Generic;

namespace DAL.Modeles;

public partial class Programmation
{
    public int ProgrammationId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Heure { get; set; }

    public string Lieu { get; set; } = null!;

    public int? SpectacleId { get; set; }

    public virtual ICollection<Billet> Billets { get; set; } = new List<Billet>();

    public virtual Spectacle? Spectacle { get; set; }
}
