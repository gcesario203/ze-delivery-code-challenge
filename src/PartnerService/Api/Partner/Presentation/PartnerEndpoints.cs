

using PartnerService.Api.Shared.DataObjects;
using PartnerService.Application.Partner.Commands.CreatePartner;
using PartnerService.Application.Partner.Queries.GetById;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Application.Shared.Exceptions;
using PartnerService.Application.Shared.ValueObjects;

namespace PartnerService.Api.Partner.Presentation;

public static class PartnerEndpoints
{
    public static void MapPartnerEndpoints(this WebApplication app)
    {
        app.MapGet("/partners/{id}", async (string id, IHandlerDispatcher handlerDispatcher) =>
        {
            var query = new GetPartnerByIdQuery
            {
                Id = Guid.TryParse(id, out var guid) ? guid : Guid.Empty
            };

            var result = await handlerDispatcher.DispatchAsync<GetPartnerByIdQuery, PartnerViewModel>(query);

            if (result == null)
            {
                return Results.NotFound(new ApiResponse<string>(null, $"No partner found with Id: {id}", false));
            }

            return Results.Ok(new ApiResponse<PartnerViewModel>(result));
        })
        .WithName("GetPartnerById")
        .WithTags("Partners");

        app.MapPost("/partners", async (CreatePartnerCommand command, IHandlerDispatcher handlerDispatcher) =>
        {
            var result = await handlerDispatcher.DispatchAsync<CreatePartnerCommand, PartnerViewModel>(command);
            return Results.Created($"/partners/{result.Id}", new ApiResponse<PartnerViewModel>(result, "Partner created successfully"));
        })
        .WithName("CreatePartner")
        .WithTags("Partners");
    }
}