using Fluxor;
using ScanApp.Application.Admin.Queries.GetAllUserData;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using System.Threading.Tasks;

namespace ScanApp.Store.Features.Admin.GetUserData
{
    public class GetUserDataEffect : Effect<GetUserDataAction>
    {
        private readonly IScopedMediator _mediator;

        public GetUserDataEffect(IScopedMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task HandleAsync(GetUserDataAction action, IDispatcher dispatcher)
        {
            var result = await _mediator.SendScoped(new GetAllUserDataQuery(action.Name)).ConfigureAwait(false);

            if (result.Conclusion)
                dispatcher.Dispatch(new GetUserDataSuccessAction(result.Output));
            else
                dispatcher.Dispatch(new GetUserDataFailureAction(result.ErrorDescription.AsError()));
        }
    }
}