using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.Depots.Commands.DeleteDepot
{
    public record DeleteDepotCommand(int Id, Version Version) : IRequest<Result>;

    internal class DeleteDepotCommandHandler : IRequestHandler<DeleteDepotCommand, Result>
    {
        private readonly IContextFactory _factory;

        public DeleteDepotCommandHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result> Handle(DeleteDepotCommand request, CancellationToken cancellationToken)
        {
            await using var ctx = _factory.CreateDbContext();
            var depot = new Depot(request.Id, "name", "0", "e@m.c", Address.Create("name", "name", "name", "name"));
            depot.ChangeVersion(request.Version);

            ctx.Remove(depot);
            var removed = await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return removed > 0 ? new Result(ResultType.Deleted) : new Result(ErrorType.NotFound);
        }
    }
}