
using System.Data;
using Dapper;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Partner.Repositories;
using PartnerService.Infra.Partner.Mappers;
using PartnerService.Infra.Shared.Models;

namespace PartnerService.Infra.Partner.Repositories.Queries;

public class PartnerQueryRepository : IPartnerQueryRepository
{
    private readonly IUnitOfWork _uow;

    public PartnerQueryRepository(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<PartnerEntity> GetByCnpjAsync(string cnpj)
    {
        var query = "SELECT * FROM Partners WHERE Document = @Cnpj";
        var parameters = new { Cnpj = cnpj };

        var dbModel = await _uow.GetDbConnection()
                                .QueryFirstOrDefaultAsync<PartnerDbModel>(query,
                                                                          parameters,
                                                                          transaction: _uow.GetDbTransaction() ?? null);

        return dbModel?.ToEntity();
    }

    public async Task<PartnerEntity> GetByIdAsync(Guid id)
    {
        var query = "SELECT * FROM Partners WHERE Id = @Id";
        var parameters = new { Id = id };

        var dbModel = await _uow.GetDbConnection()
                                .QueryFirstOrDefaultAsync<PartnerDbModel>(query,
                                                                          parameters,
                                                                          transaction: _uow.GetDbTransaction() ?? null);

        return dbModel?.ToEntity();
    }
}