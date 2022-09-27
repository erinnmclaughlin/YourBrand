using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Domain;

public interface IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}