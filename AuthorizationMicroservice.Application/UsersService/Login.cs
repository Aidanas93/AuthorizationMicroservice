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
    public class Login
    {
        public class Command : IRequest<AccessTokenDto>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }


        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Email).EmailAddress().NotNull().Length(3, 50);
                RuleFor(x => x.Password).Length(3, 25).NotNull().NotEmpty();
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
                    UserCredential user = await unitOfWork.UserRepository.LoadRecordByEmailAsync(request.Email);

                    if (!LoginHandler.ValidateUser(request.Password, user.PasswordSalt, user.PasswordHash))
                        throw new StatusCodeException(HttpStatusCode.Unauthorized, $"User was not authorized", user.Id);


                    if (!unitOfWork.JWTHandler.IsValid(user.AccessToken.JWTToken))
                    {
                        user.AccessToken.JWTToken = unitOfWork.JWTHandler.CreateJWTToken(user);
                        await unitOfWork.UserRepository.UpsertRecordAsync(user.Id, user);
                    }
                    unitOfWork.Logger.LogInformation($"Successful login with user id: {user.Id}, {HttpStatusCode.OK}");
                    AccessTokenDto token = new AccessTokenDto()
                    {
                        Id = user.Id,
                        Token = user.AccessToken.JWTToken
                    };

                    return token;

                }
                catch (InvalidOperationException)
                {
                    throw new StatusCodeException(HttpStatusCode.NotFound, $"User was not found");
                }

            }
        }
    }
}
