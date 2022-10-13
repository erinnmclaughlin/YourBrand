using YourBrand.Warehouse.Domain.Common;

namespace YourBrand.Warehouse.Domain.Events;

public record ItemCreated(string ItemsId) : DomainEvent;

public record ItemQuantityOnHandUpdated(string ItemId, int Quantity) : DomainEvent;

public record ItemsPicked(string ItemId, int Quantity) : DomainEvent;

public record ItemsReserved(string ItemId, int Quantity) : DomainEvent;

public record ItemQuantityAvailableUpdated(string ItemId, int Quantity) : DomainEvent;