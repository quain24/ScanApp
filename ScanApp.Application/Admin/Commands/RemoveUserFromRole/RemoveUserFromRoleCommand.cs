using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.RemoveUserFromRole
{
    /// <summary>
    /// Represents a command used to request removal of a user with given <paramref name="UserName"/> from a role with given <paramref name="RoleName"/>
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="UserName">Name of user to be removed from role.</param>
    /// <param name="Version">User's <see cref="ScanApp.Domain.ValueObjects.Version">Version</see> to be compared to a concurrency stamp in data source.</param>
    /// <param name="RoleName">Name of role from which user should be removed.</param>
    public record RemoveUserFromRoleCommand(string UserName, Version Version, string RoleName) : IRequest<Result<Version>>;

    internal class RemoveUserFromRoleCommandHandler : IRequestHandler<RemoveUserFromRoleCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;
        private readonly IContextFactory _factory;

        public RemoveUserFromRoleCommandHandler(IUserManager userManager, IRoleManager roleManager, IContextFactory factory)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<Version>> Handle(RemoveUserFromRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await using var context = _factory.CreateDbContext();
                var strategy = context.Database.CreateExecutionStrategy();

                return await strategy.ExecuteAsync(async token =>
                {
                    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                    await using var ctx = _factory.CreateDbContext();
                    token.ThrowIfCancellationRequested();
                    var result = await _userManager.RemoveUserFromRole(request.UserName, request.Version, request.RoleName).ConfigureAwait(false);

                    if (request.RoleName.Equals(Globals.RoleNames.Administrator))
                    {
                        var adminUsers = await _roleManager.UsersInRole(Globals.RoleNames.Administrator, cancellationToken).ConfigureAwait(false);
                        if (adminUsers.Count == 0)
                        {
                            return new Result<Version>(ErrorType.IllegalAccountOperation,
                                $"Cannot remove user from {Globals.RoleNames.Administrator} role - no more users with " +
                                $"{Globals.RoleNames.Administrator} role would be left.").SetOutput(result.Output);
                        }
                    }

                    scope.Complete();
                    return result;
                }, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                return new Result<Version>(ErrorType.Cancelled, ex).SetOutput(request.Version);
            }
            catch (DbUpdateException ex)
            {
                return ex is DbUpdateConcurrencyException
                    ? new Result<Version>(ErrorType.ConcurrencyFailure, ex.InnerException?.Message ?? ex.Message, ex).SetOutput(request.Version)
                    : new Result<Version>(ErrorType.DatabaseError, ex.InnerException?.Message ?? ex.Message, ex).SetOutput(request.Version);
            }
            catch (SqlException ex)
            {
                return new Result<Version>(ErrorType.DatabaseError, ex.InnerException?.Message ?? ex.Message, ex).SetOutput(request.Version);
            }
        }
    }
}