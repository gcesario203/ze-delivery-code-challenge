

using System.Linq.Expressions;

namespace PartnerService.Domain.Shared.Repositories;

public interface IQueryRepository<T>
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetByFilters(Expression<Func<T, bool>> filter);
}