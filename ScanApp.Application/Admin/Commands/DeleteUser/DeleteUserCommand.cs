using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace ScanApp.Application.Admin.Commands.DeleteUser
{
    /// <summary>
    /// Represents a command used to request deletion of a user with given <paramref name="UserName"/>
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="UserName">Name of user to be deleted.</param>
    public record DeleteUserCommand(string UserName) : IRequest<Result>;

    internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;
        private readonly IContextFactory _factory;

        public DeleteUserCommandHandler(IUserManager userManager, IRoleManager roleManager, IContextFactory factory)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await using var context = _factory.CreateDbContext();
            var strategy = context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async token =>
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                await using var ctx = _factory.CreateDbContext();
                token.ThrowIfCancellationRequested();
                var result = await _userManager.DeleteUser(request.UserName).ConfigureAwait(false);

                var adminUsers = await _roleManager.UsersInRole(Globals.RoleNames.Administrator, cancellationToken).ConfigureAwait(false);
                if (adminUsers.Count == 0)
                {
                    return new Result<Version>(ErrorType.IllegalAccountOperation,
                        $"Cannot delete user {request.UserName} " +
                        $"there would be no {Globals.RoleNames.Administrator} users left.").SetOutput(result.Output);
                }

                scope.Complete();
                return result;
            }, cancellationToken);
        }
    }
}