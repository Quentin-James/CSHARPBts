using System;
using System.Collections.Generic;

namespace WebApplication1.Modeles;

public partial class Spectacle
{
    public int SpectacleId { get; set; }

    public string Titre { get; set; } = null!;

    public string? Description { get; set; }

    public string? Type { get; set; }

    public TimeOnly? Duree { get; set; }

    public string? Saison { get; set; }

    public int? SpectacleEnfant1Id { get; set; }

    public int? SpectacleEnfant2Id { get; set; }

    public int? SpectacleEnfant3Id { get; set; }

    public bool? DeconseilleAuxEnfants { get; set; }

    public virtual ICollection<Spectacle> InverseSpectacleEnfant1 { get; set; } = new List<Spectacle>();

    public virtual ICollection<Spectacle> InverseSpectacleEnfant2 { get; set; } = new List<Spectacle>();

    public virtual ICollection<Spectacle> InverseSpectacleEnfant3 { get; set; } = new List<Spectacle>();

    public virtual ICollection<Programmation> Programmations { get; set; } = new List<Programmation>();

    public virtual Spectacle? SpectacleEnfant1 { get; set; }

    public virtual Spectacle? SpectacleEnfant2 { get; set; }

    public virtual Spectacle? SpectacleEnfant3 { get; set; }

    public virtual ICollection<TarifsSpectacle> TarifsSpectacles { get; set; } = new List<TarifsSpectacle>();

    public virtual ICollection<Artiste> Artistes { get; set; } = new List<Artiste>();

    public virtual ICollection<GroupesSpectacle> Groupes { get; set; } = new List<GroupesSpectacle>();
}
