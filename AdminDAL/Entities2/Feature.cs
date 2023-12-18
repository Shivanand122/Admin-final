using System;
using System.Collections.Generic;

namespace AdminDAL.Entities2;

public partial class Feature
{
    public int FeatureId { get; set; }

    public string FeatureName { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string FeatureDataType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public byte ApprovalStatus { get; set; }

    public string? AdminComments { get; set; }

    public string EntityName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public virtual EntityTbl EntityNameNavigation { get; set; } = null!;
}
