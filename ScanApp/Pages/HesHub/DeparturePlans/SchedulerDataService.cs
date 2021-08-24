using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Pages.HesHub.DeparturePlans
{
    public class SchedulerDataService
    {
        private IMediator Mediator { get; }

        public SchedulerDataService(IMediator mediator)
        {
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
    }

    
}
