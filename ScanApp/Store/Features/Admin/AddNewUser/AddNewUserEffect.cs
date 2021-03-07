using Fluxor;
using ScanApp.Application.Admin.Commands.AddUser;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using System.Threading.Tasks;

namespace ScanApp.Store.Features.Admin.AddNewUser
{
    public class AddNewUserEffect : Effect<AddNewUserAction>
    {
        private readonly IScopedMediator _mediator;

        public AddNewUserEffect(IScopedMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task HandleAsync(AddNewUserAction action, IDispatcher dispatcher)
        {
            var result = await _mediator.SendScoped(new AddUserCommand(action.UserModel)).ConfigureAwait(false);

            if (result.Conclusion)
                dispatcher.Dispatch(new AddNewUserSuccessAction(action.UserModel.Name));
            else
                dispatcher.Dispatch(new AddNewUserFailureAction(result.ErrorDescription.AsError()));
        }
    }
}