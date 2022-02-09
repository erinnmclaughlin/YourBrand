﻿
using MediatR;

using Microsoft.AspNetCore.Mvc;

using Skynet.TimeReport.Application.Common.Interfaces;
using Skynet.TimeReport.Application.Reports.Queries;
using Skynet.TimeReport.Infrastructure;

namespace Skynet.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<FileStreamResult> GetReport([FromQuery] string[] projectIds, [FromQuery] string? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
    {
        var stream = await _mediator.Send(new CreateReportCommand(projectIds, userId, startDate, endDate), cancellationToken);

        if (stream is null) throw new Exception();

        return File(stream, "application/vnd.ms-excel", "TimeReport.xlsx");
    }
}