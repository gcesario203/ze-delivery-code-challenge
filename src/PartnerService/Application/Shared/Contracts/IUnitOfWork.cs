
using System.Data;

namespace PartnerService.Application.Shared.Contracts;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();

    IDbConnection GetDbConnection();

    IDbTransaction GetDbTransaction();
}