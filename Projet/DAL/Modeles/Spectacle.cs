using System;
using System.Collections.Generic;

namespace DAL.Modeles;

public partial class Spectacle
{
    public int SpectacleId { get; set; }

    public string Titre { get; set; } = null!;

    public string? Description { get; set; }

    public string? Type { get; set; }

    public TimeOnly? Duree { get; set; }

    public virtual ICollection<Programmation> Programmations { get; set; } = new List<Programmation>();

    public virtual ICollection<TarifsSpectacle> TarifsSpectacles { get; set; } = new List<TarifsSpectacle>();

    public virtual ICollection<Artiste> Artistes { get; set; } = new List<Artiste>();

    public virtual ICollection<GroupesSpectacle> Groupes { get; set; } = new List<GroupesSpectacle>();
}
