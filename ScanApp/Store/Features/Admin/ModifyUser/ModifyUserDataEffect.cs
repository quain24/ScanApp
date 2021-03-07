using Fluxor;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using System.Threading.Tasks;

namespace ScanApp.Store.Features.Admin.ModifyUser
{
    public class ModifyUserDataEffect : Effect<ModifyUserDataAction>
    {
        private readonly IScopedMediator _mediator;

        public ModifyUserDataEffect(IScopedMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task HandleAsync(ModifyUserDataAction action, IDispatcher dispatcher)
        {
            var data = action.UserData;
            var result = await _mediator.SendScoped(new EditUserDataCommand(data.Name)
            {
                ConcurrencyStamp = data.Version,
                Phone = data.Phone,
                Email = data.Email,
                Location = data.Location,
                NewName = data.NewName
            }).ConfigureAwait(false);

            if (result.Conclusion)
            {
                data.Version = result.Output;
                dispatcher.Dispatch(new ModifyUserDataSuccessAction(data));
            }
            else
                dispatcher.Dispatch(new ModifyUserDataFailureAction(result.ErrorDescription.AsError()));
        }
    }
}