using System;
using System.Collections.Generic;

namespace WebApplication1.Modeles;

public partial class TarifsGroupe
{
    public int TarifId { get; set; }

    public int GroupeId { get; set; }

    public decimal Prix { get; set; }

    public virtual GroupesSpectacle Groupe { get; set; } = null!;

    public virtual TypesTarif Tarif { get; set; } = null!;
}
