﻿using System;
using System.Runtime.Serialization;

namespace Skynet.Showroom.Application.ConsultantProfiles;

public abstract class ConsultantProfileViewModelDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string DisplayName { get; set; }

    public string OrganizationId { get; set; }

    public string CompetenceAreaId { get; set; }

    public string ManagerId { get; set; }

    public string Headline { get; set; }

    public string ShortPresentation { get; set; }

    public string Presentation { get; set; }

    public DateTime? AvailableFromDate { get; set; }
}
