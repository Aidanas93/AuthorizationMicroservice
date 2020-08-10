using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Application.Infrastructure
{
    public class ApplicationLogger
    {
        public class Event : INotification
        {
            public string Message { get; set; }
        }

        public class Handler : INotificationHandler<Event>
        {
            private readonly UnitOfWork unitOfWork;

            public Handler(UnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task Handle(Event notification, CancellationToken cancellationToken)
            {
                unitOfWork.Logger.LogInformation(notification.Message);
            }
        }
    }
}
