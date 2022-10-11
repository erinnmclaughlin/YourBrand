﻿using YourBrand.Showroom.Domain.Common;
using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Domain.Entities;

public class Case : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public CaseStatus Status { get; set; }

    public ICollection<CaseProfile> CaseProfiles { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}

public class Location : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public string? CityOrDistrict { get; set; }

    public string? Cou { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
