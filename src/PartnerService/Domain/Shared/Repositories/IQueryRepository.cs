

using System.Linq.Expressions;

namespace PartnerService.Domain.Shared.Repositories;

public interface IQueryRepository<T>
{
    Task<T> GetByIdAsync(Guid id);
}