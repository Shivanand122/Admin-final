using System;
using System.Collections.Generic;

namespace AdminDAL.Entities2;

public partial class EntityTbl
{
    public string EntityName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Feature> Features { get; set; } = new List<Feature>();
}
