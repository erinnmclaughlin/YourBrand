using MediatR;

namespace YourBrand.Marketing.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : Domain.DomainEvent
{

}