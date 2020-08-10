using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Application.Infrastructure;
using AuthorizationMicroservice.Application.Infrastructure.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Application.UsersService
{
    public class Edit
    {
        public class Command : IRequest<AccessTokenDto>
        {
            public Guid Id { get; set; }
            public string JwtToken { get; set; }
            public string Email { get; set; }
            public UserInfoDto UserInfo { get; set; } = new UserInfoDto();
        }


        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Email).EmailAddress().NotNull().Length(3, 50);
                RuleFor(x => x.UserInfo.Firstname).Length(1, 25).NotEmpty().NotNull();
                RuleFor(x => x.UserInfo.Lastname).Length(1, 50).NotEmpty().NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, AccessTokenDto>
        {
            private readonly UnitOfWork unitOfWork;

            public Handler(UnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }
            public async Task<AccessTokenDto> Handle(Command request, CancellationToken cancellationToken)
            {

                try
                {
                    string token = unitOfWork.JWTHandler.RemoveBearerFromJWTToken(request.JwtToken);
                    var tokenUserId = unitOfWork.JWTHandler.TokenId(token);
                    if (tokenUserId == request.Id)
                    {
                        var user = await unitOfWork.UserRepository.LoadRecordByIdAsync(request.Id);

                        user.Email = request.Email;
                        user.UserInfo.Firstname = request.UserInfo.Firstname;
                        user.UserInfo.Lastname = request.UserInfo.Lastname;
                        user.UserInfo.PictureUrl = request.UserInfo.PictureUrl;

                        await unitOfWork.UserRepository.UpsertRecordAsync(request.Id, user);

                        unitOfWork.Logger.LogInformation($"Successful edited user with id: {user.Id}, HTTP {HttpStatusCode.OK}");

                        return new AccessTokenDto()
                        {
                            Id = request.Id,
                            Token = user.AccessToken.JWTToken
                        };
                    }
                    else
                    {
                        throw new StatusCodeException(HttpStatusCode.Unauthorized, $"User was not authorized", request.Id);
                    }
                }
                catch (MongoWriteException)
                {
                    throw new StatusCodeException(HttpStatusCode.Conflict, "User with this email already exists");
                }

            }
        }

    }
}
