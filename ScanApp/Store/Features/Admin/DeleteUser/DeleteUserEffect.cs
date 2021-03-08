using Fluxor;
using ScanApp.Application.Admin.Commands.DeleteUser;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using System.Threading.Tasks;

namespace ScanApp.Store.Features.Admin.DeleteUser
{
    public class DeleteUserEffect : Effect<DeleteUserAction>
    {
        private readonly IScopedMediator _mediator;
        private readonly IState<AdminState> _state;

        public DeleteUserEffect(IScopedMediator mediator, IState<AdminState> state)
        {
            _mediator = mediator;
            _state = state;
        }

        public override async Task HandleAsync(DeleteUserAction action, IDispatcher dispatcher)
        {
            var result = await _mediator.SendScoped(new DeleteUserCommand(_state.Value.SelectedUserName, _state.Value.SelectedUserVersion))
                .ConfigureAwait(false);

            if (result.Conclusion || result.ErrorDescription.ErrorType.Equals(ErrorType.NotFound))
                dispatcher.Dispatch(new DeleteUserSuccessAction(_state.Value.SelectedUserName));
            else
                dispatcher.Dispatch(new DeleteUserFailureAction(result.ErrorDescription.AsError()));
        }
    }
}