using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.Depots.Commands.CreateNewDepot
{
    public record CreateNewDepotCommand(DepotModel Model) : IRequest<Result<Version>>;

    internal class CreateNewDepotCommandHandler : IRequestHandler<CreateNewDepotCommand, Result<Version>>
    {
        private readonly IContextFactory _factory;

        public CreateNewDepotCommandHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<Version>> Handle(CreateNewDepotCommand request, CancellationToken cancellationToken)
        {
            await using var ctx = _factory.CreateDbContext();

            var model = request.Model;
            var (gate, trailer) = CreateChildren(model);
            var depot = new Depot(model.Id, model.Name, model.PhoneNumber, model.Email,
                Address.Create(model.StreetName, model.ZipCode, model.City, model.Country))
            {
                DefaultGate = gate,
                DefaultTrailer = trailer
            };

            await ctx.Depots.AddAsync(depot, cancellationToken).ConfigureAwait(false);
            if (depot.DefaultGate is not null)
                ctx.Entry(depot.DefaultGate).State = EntityState.Unchanged;
            if (depot.DefaultTrailer is not null)
                ctx.Entry(depot.DefaultTrailer).State = EntityState.Unchanged;

            var saved = await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return saved >= 1 ? new Result<Version>(ResultType.Created).SetOutput(depot.Version) : new Result<Version>(ErrorType.Unknown);
        }

        private static (Gate Gate, TrailerType Trailer) CreateChildren(DepotModel model)
        {
            var gate = model.DefaultGate is null
                ? null
                : new Gate(-100, 0)
                {
                    Version = model.DefaultGate.Version,
                    Id = model.DefaultGate.Id
                };
            var trailer = model.DefaultTrailer is null
                ? null
                : new TrailerType("valid")
                {
                    Version = model.DefaultTrailer.Version,
                    Id = model.DefaultTrailer.Id
                };

            return (gate, trailer);
        }
    }
}