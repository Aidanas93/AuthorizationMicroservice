using AuthorizationMicroservice.Application.CryptographyService;
using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Application.Infrastructure;
using AuthorizationMicroservice.Application.Infrastructure.Exceptions;
using AuthorizationMicroservice.Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Application.UsersService
{
    public class EditPassword
    {
        public class Command : IRequest<AccessTokenDto>
        {
            public Guid Id { get; set; }
            public string JwtToken { get; set; }
            public string Password { get; set; }
            public string NewPassword { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Password).Length(3, 25).NotNull().NotEmpty();
                RuleFor(x => x.NewPassword).Length(3, 25).NotNull().NotEmpty();
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

                string token = unitOfWork.JWTHandler.RemoveBearerFromJWTToken(request.JwtToken);
                var tokenUserId = unitOfWork.JWTHandler.TokenId(token);
                if (tokenUserId == request.Id)
                {
                    var user = await unitOfWork.UserRepository.LoadRecordByIdAsync(request.Id);

                    await ChangePassword(request, user);
                    unitOfWork.Logger.LogInformation($"Successfuly edited user password with id: {user.Id}, HTTP {HttpStatusCode.OK}");

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

            private async Task ChangePassword(Command request, UserCredential user)
            {
                if (LoginHandler.ValidateUser(request.Password, user.PasswordSalt, user.PasswordHash))
                {
                    user.PasswordSalt = SaltHandler.Create();
                    user.PasswordHash = HashHandler.Create(request.NewPassword, user.PasswordSalt);
                    await unitOfWork.UserRepository.UpsertRecordAsync(request.Id, user);

                }
                else
                {
                    throw new StatusCodeException(HttpStatusCode.Unauthorized, $"User was not authorized", request.Id);
                }
            }
        }
    }
}
