
using AspNetCore.Authentication.ApiKey;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.HumanResources.Application.Common.Models;
using YourBrand.HumanResources.Application.Persons;
using YourBrand.HumanResources.Application.Persons.Commands;
using YourBrand.HumanResources.Application.Persons.Queries;
using YourBrand.HumanResources.Domain.Exceptions;

namespace YourBrand.HumanResources;

[Route("[controller]")]
[ApiController]
public class PersonsController : Controller
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<PersonDto>>> GetPersons(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, HumanResources.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetPersonsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PersonDto>> GetPerson(string id, CancellationToken cancellationToken)
    {
        var person = await _mediator.Send(new GetPersonQuery(id), cancellationToken);

        if (person is null)
        {
            return NotFound();
        }

        return Ok(person);
    }

    [HttpGet("{id}/Roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<RoleDto>>> GetPersonRoles(string id, int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, HumanResources.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetPersonRolesQuery(id, page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PersonDto>> CreatePerson(CreatePersonDto createPersonDto, CancellationToken cancellationToken)
    {
        try
        {
            var person = await _mediator.Send(new CreatePersonCommand(createPersonDto.FirstName, createPersonDto.LastName, createPersonDto.DisplayName, createPersonDto.Role, createPersonDto.SSN, createPersonDto.Email, null, createPersonDto.Password), cancellationToken);

            return Ok(person);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("{id}/Details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PersonDto>> UpdatePerson(string id, UpdatePersonDetailsDto updatePersonDetailsDto, CancellationToken cancellationToken)
    {
        try
        {
            var person = await _mediator.Send(new UpdatePersonDetailsCommand(id, updatePersonDetailsDto.FirstName, updatePersonDetailsDto.LastName, updatePersonDetailsDto.DisplayName, updatePersonDetailsDto.SSN, updatePersonDetailsDto.Email), cancellationToken);

            return Ok(person);
        }
        catch (PersonNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpPost("{id}/Role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> UpdateRole(string id, UpdatePersonRoleDto updatePersonRoleDtoDto, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new UpdatePersonRoleCommand(id, updatePersonRoleDtoDto.Role), cancellationToken);

            return Ok();
        }
        catch (PersonNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeletePerson(string id, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new DeletePersonCommand(id), cancellationToken);

            return Ok();
        }
        catch (PersonNotFoundException)
        {
            return NotFound();
        }
    }
}

public record class CreatePersonDto(string FirstName, string LastName, string? DisplayName, string Role, string SSN, string Email, string Password);

public record class UpdatePersonDetailsDto(string FirstName, string LastName, string? DisplayName, string SSN, string Email);

public record class ChangePasswordDto(string CurrentPassword, string NewPassword);

public record class UpdatePersonRoleDto(string Role);