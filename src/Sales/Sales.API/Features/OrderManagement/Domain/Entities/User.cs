namespace YourBrand.Sales.Features.OrderManagement.Domain.Entities;

public class User : AggregateRoot<string>, IAuditable
{
    public User(string id, string organizationId, string name, string email)
        : base(id)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        Email = email;
    }

    public string Name { get; set; }

    public Organization Organization { get; set; }

    public string OrganizationId { get; set; }

    public string Email { get; set; }

    public User? CreatedBy { get; set; }

    public string? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
