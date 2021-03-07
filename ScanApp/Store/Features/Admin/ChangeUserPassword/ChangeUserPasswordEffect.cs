using Fluxor;
using ScanApp.Application.Admin.Commands.ChangeUserPassword;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using System.Threading.Tasks;

namespace ScanApp.Store.Features.Admin.ChangeUserPassword
{
    public class ChangeUserPasswordEffect : Effect<ChangeUserPasswordAction>
    {
        private readonly IScopedMediator _mediator;
        private readonly IState<AdminState> _state;

        public ChangeUserPasswordEffect(IScopedMediator mediator, IState<AdminState> state)
        {
            _mediator = mediator;
            _state = state;
        }

        public override async Task HandleAsync(ChangeUserPasswordAction action, IDispatcher dispatcher)
        {
            var result = await _mediator.SendScoped(new ChangeUserPasswordCommand(_state.Value.SelectedUserName,
                action.NewPassword, _state.Value.SelectedUserVersion)).ConfigureAwait(false);

            if (result.Conclusion)
                dispatcher.Dispatch(new ChangeUserPasswordSuccessAction(result.Output));
            else
                dispatcher.Dispatch(new ChangeUserPasswordFailureAction(result.ErrorDescription.AsError()));
        }
    }
}