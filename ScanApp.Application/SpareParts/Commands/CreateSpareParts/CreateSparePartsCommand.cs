using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.SpareParts.Commands.CreateSpareParts
{
    public record CreateSparePartsCommand(params SparePartModel[] SpareParts) : IRequest<Result>;

    internal class CreateSparePartsCommandHandler : IRequestHandler<CreateSparePartsCommand, Result>
    {
        private readonly IContextFactory _contextFactory;

        public CreateSparePartsCommandHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result> Handle(CreateSparePartsCommand request, CancellationToken cancellationToken)
        {
            await using var ctx = _contextFactory.CreateDbContext();

            try
            {
                var spareParts = request.SpareParts.Select(s =>
                    new SparePart(s.Name, s.Amount, s.SourceArticleId, s.SparePartStoragePlaceId));

                await ctx.SpareParts.AddRangeAsync(spareParts, cancellationToken).ConfigureAwait(false);
                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return new Result(ResultType.Created);
            }
            catch (DbUpdateException ex)
            {
                return ex is DbUpdateConcurrencyException
                    ? new Result(ErrorType.ConcurrencyFailure, ex)
                    : new Result(ErrorType.DatabaseError, ex);
            }
        }
    }
}