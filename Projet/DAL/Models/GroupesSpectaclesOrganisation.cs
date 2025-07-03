using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class GroupesSpectaclesOrganisation
{
    public int GroupeId { get; set; }

    public string? TypeSpectacle { get; set; }

    public TimeOnly? Duree { get; set; }

    public string? Description { get; set; }

    public virtual GroupesSpectacle Groupe { get; set; } = null!;
}
