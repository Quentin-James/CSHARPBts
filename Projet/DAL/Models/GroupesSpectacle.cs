using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class GroupesSpectacle
{
    public int GroupeId { get; set; }

    public string NomGroupe { get; set; } = null!;

    public virtual GroupesSpectaclesOrganisation? GroupesSpectaclesOrganisation { get; set; }

    public virtual ICollection<TarifsGroupe> TarifsGroupes { get; set; } = new List<TarifsGroupe>();

    public virtual ICollection<Spectacle> Spectacles { get; set; } = new List<Spectacle>();
}
