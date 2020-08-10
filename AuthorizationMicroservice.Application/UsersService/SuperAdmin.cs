using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Application.Infrastructure;
using AuthorizationMicroservice.Application.Infrastructure.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Application.UsersService
{
    public class SuperAdmin
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string JwtToken { get; set; }
            public string Email { get; set; }
            public AdminUserInfoDto UserInfo { get; set; } = new AdminUserInfoDto();
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Email).EmailAddress().NotNull().Length(3, 50);
                RuleFor(x => x.UserInfo.Firstname).Length(1, 25).NotEmpty().NotNull();
                RuleFor(x => x.UserInfo.Lastname).Length(1, 50).NotEmpty().NotNull();
                RuleFor(x => x.UserInfo.Role).NotEmpty().NotNull();
            }
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
                    var user = await unitOfWork.UserRepository.LoadRecordByIdAsync(request.Id);

                    user.Email = request.Email;
                    user.UserInfo.Firstname = request.UserInfo.Firstname;
                    user.UserInfo.Lastname = request.UserInfo.Lastname;
                    user.UserInfo.PictureUrl = request.UserInfo.PictureUrl;
                    user.UserInfo.Role = request.UserInfo.Role;

                    user.AccessToken.JWTToken = unitOfWork.JWTHandler.CreateJWTToken(user);
                    await unitOfWork.UserRepository.UpsertRecordAsync(user.Id, user);
                    unitOfWork.Logger.LogInformation($"User with id: {user.Id} was edited by SuperAdmin");
                    return Unit.Value;
                }
                catch (InvalidOperationException)
                {
                    unitOfWork.Logger.LogInformation($"Unsuccessful user edit with id: {request.Id} by SuperAdmin");
                    throw new StatusCodeException(HttpStatusCode.NotFound, $"User is not found");
                }

            }
        }
    }
}
