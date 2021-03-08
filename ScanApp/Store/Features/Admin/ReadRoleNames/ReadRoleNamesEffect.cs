using Fluxor;
using Microsoft.Extensions.Logging;
using ScanApp.Application.Admin.Queries.GetAllUserRoles;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using System.Threading.Tasks;

namespace ScanApp.Store.Features.Admin.ReadRoleNames
{
    public class ReadRoleNamesEffect : LoggingEffect<ReadRoleNamesAction>
    {
        private readonly IScopedMediator _mediator;

        public ReadRoleNamesEffect(IScopedMediator mediator, ILogger<ReadRoleNamesEffect> logger) : base(nameof(ReadRoleNamesAction), logger)
        {
            _mediator = mediator;
        }

        public override async Task LoggingHandleAsync(ReadRoleNamesAction action, IDispatcher dispatcher)
        {
            var result = await _mediator.SendScoped(new GetAllUserRolesQuery()).ConfigureAwait(false);

            if (result.Conclusion)
                dispatcher.Dispatch(new ReadRoleNamesSuccessAction(result.Output));
            else
                dispatcher.Dispatch(new ReadRoleNamesFailureNames(result.ErrorDescription?.AsError()));
        }
    }
}