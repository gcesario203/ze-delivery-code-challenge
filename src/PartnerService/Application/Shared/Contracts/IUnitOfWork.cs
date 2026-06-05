
using System.Data;

namespace PartnerService.Application.Shared.Contracts;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();

    IDbConnection GetDbConnection();

    IDbTransaction GetDbTransaction();
}