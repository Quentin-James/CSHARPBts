using System;
using System.Collections.Generic;

namespace WebApplication1.Modeles;

public partial class TarifsSpectacle
{
    public int TarifId { get; set; }

    public int SpectacleId { get; set; }

    public decimal Prix { get; set; }

    public virtual Spectacle Spectacle { get; set; } = null!;

    public virtual TypesTarif Tarif { get; set; } = null!;
}
