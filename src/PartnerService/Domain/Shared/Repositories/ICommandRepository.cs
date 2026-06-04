
namespace PartnerService.Domain.Shared.Repositories;

public interface ICommandRepository<T>
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}