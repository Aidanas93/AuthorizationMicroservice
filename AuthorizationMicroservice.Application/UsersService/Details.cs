using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Application.Infrastructure;
using AuthorizationMicroservice.Application.Infrastructure.Exceptions;
using AuthorizationMicroservice.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Application.UsersService
{
    public class Details
    {
        public class Query : IRequest<UserCredentialDto>
        {
            public Guid Id { get; set; }

            public string JwtToken { get; set; }
        }

        public class Handler : IRequestHandler<Query, UserCredentialDto>
        {
            private readonly UnitOfWork unitOfWork;

            public Handler(UnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<UserCredentialDto> Handle(Query request, CancellationToken cancellationToken)
            {

                string token = unitOfWork.JWTHandler.RemoveBearerFromJWTToken(request.JwtToken);
                var tokenUserId = unitOfWork.JWTHandler.TokenId(token);
                if (tokenUserId == request.Id)
                {
                    var user = await unitOfWork.UserRepository.LoadRecordByIdAsync(request.Id);

                    if (user == null)
                        throw new StatusCodeException(HttpStatusCode.NotFound, $"User is not found", user.Id);

                    unitOfWork.Logger.LogInformation($"Get User by id: {user.Id}, HTTP 200");

                    return new UserCredentialDto
                    {
                        AccessToken = new AccessToken { JWTToken = token },
                        Email = user.Email,
                        Id = user.Id,
                        UserInfo = user.UserInfo
                    };

                }
                else
                {
                    unitOfWork.Logger.LogInformation($"User is not found with id: {request.Id}, HTTP 404");
                    throw new StatusCodeException(HttpStatusCode.Unauthorized, $"User was not authorized", request.Id);
                }
            }
        }
    }
}
