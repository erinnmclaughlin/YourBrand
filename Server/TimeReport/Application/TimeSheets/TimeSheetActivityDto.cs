﻿using Skynet.TimeReport.Application.Projects;

namespace Skynet.TimeReport.Application.TimeSheets;

public record class TimeSheetActivityDto(string Id, string Name, string? Description, ProjectDto Project, IEnumerable<TimeSheetEntryDto> Entries);