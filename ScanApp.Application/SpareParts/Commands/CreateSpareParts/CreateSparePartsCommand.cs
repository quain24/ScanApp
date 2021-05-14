using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.SpareParts.Commands.CreateSpareParts
{
    /// <summary>
    /// Represents a command used to create new spare part(s) in application's data source
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="SpareParts">Spare Parts to be inserted into data source.</param>
    public record CreateSparePartsCommand(params SparePartModel[] SpareParts) : IRequest<Result>;

    internal class CreateSparePartsCommandHandler : IRequestHandler<CreateSparePartsCommand, Result>
    {
        private readonly IContextFactory _contextFactory;

        public CreateSparePartsCommandHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result> Handle(CreateSparePartsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _contextFactory.CreateDbContext();
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
            catch (OperationCanceledException ex)
            {
                return new Result(ErrorType.Cancelled, ex);
            }
        }
    }
}