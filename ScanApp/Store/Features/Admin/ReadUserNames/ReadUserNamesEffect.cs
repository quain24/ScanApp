using Fluxor;
using ScanApp.Application.Admin.Queries.GetAllUsers;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using System.Threading.Tasks;

namespace ScanApp.Store.Features.Admin.ReadUserNames
{
    public class ReadUserNamesEffect : Effect<ReadUserNamesAction>
    {
        private readonly IScopedMediator _mediator;

        public ReadUserNamesEffect(IScopedMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task HandleAsync(ReadUserNamesAction action, IDispatcher dispatcher)
        {
            var result = await _mediator.SendScoped(new GetAllUsersQuery()).ConfigureAwait(false);
            if (result.Conclusion)
            {
                dispatcher.Dispatch(new ReadUserNamesSuccessAction(result.Output.ConvertAll(u => u.UserName)));
                return;
            }

            dispatcher.Dispatch(new ReadUserNamesFailureAction(result.ErrorDescription?.AsError()));
        }
    }
}