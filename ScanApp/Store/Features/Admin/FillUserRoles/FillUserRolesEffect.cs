using Fluxor;
using ScanApp.Application.Admin.Queries.GetUserRoles;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using System.Threading.Tasks;
using ScanApp.Application.Admin.Queries.GetUserVersion;

namespace ScanApp.Store.Features.Admin.FillUserRoles
{
    public class FillUserRolesEffect : Effect<FillUserRolesAction>
    {
        private readonly IScopedMediator _mediator;
        private readonly IState<AdminState> _state;

        public FillUserRolesEffect(IScopedMediator mediator, IState<AdminState> state)
        {
            _mediator = mediator;
            _state = state;
        }

        public override async Task HandleAsync(FillUserRolesAction action, IDispatcher dispatcher)
        {
            var result = await _mediator.SendScoped(new GetUserRolesQuery(action.UserName)).ConfigureAwait(false);
            var versionResult = await _mediator.SendScoped(new GetUserVersionCommand(action.UserName));


            if (result.Conclusion && versionResult.Conclusion)
                dispatcher.Dispatch(new FillUserRolesSuccessAction(result.Output, versionResult.Output));
            else
                dispatcher.Dispatch(new FillUserRolesFailureAction(result.ErrorDescription.AsError()));
        }
    }
}