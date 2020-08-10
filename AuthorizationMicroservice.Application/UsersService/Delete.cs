using AuthorizationMicroservice.Application.Infrastructure;
using AuthorizationMicroservice.Application.Infrastructure.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Application.UsersService
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string JwtToken { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly UnitOfWork unitOfWork;

            public Handler(UnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    string token = unitOfWork.JWTHandler.RemoveBearerFromJWTToken(request.JwtToken);
                    var tokenUserId = unitOfWork.JWTHandler.TokenId(token);
                    if (tokenUserId == request.Id || unitOfWork.JWTHandler.HasAdminRole(token))
                    {
                        var user = await unitOfWork.UserRepository.LoadRecordByIdAsync(request.Id);

                        await unitOfWork.UserRepository.DeleteRecordAsync(user.Id);
                        unitOfWork.Logger.LogInformation($"Successfuly deleted user with id: {user.Id}");

                        return Unit.Value;
                    }
                    else
                    {
                        unitOfWork.Logger.LogInformation($"Login was unauthorized");
                        throw new StatusCodeException(HttpStatusCode.Unauthorized, $"User was not authorized", request.Id);
                    }

                }
                catch (InvalidOperationException)
                {
                    throw new StatusCodeException(HttpStatusCode.NotFound, $"User is not found");
                }

            }
        }
    }
}
