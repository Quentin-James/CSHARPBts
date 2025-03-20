using System;
using System.Collections.Generic;

namespace DAL.Modeles;

public partial class Artiste
{
    public int ArtisteId { get; set; }

    public string Nom { get; set; } = null!;

    public virtual ICollection<Spectacle> Spectacles { get; set; } = new List<Spectacle>();
}
