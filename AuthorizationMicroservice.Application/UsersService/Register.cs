using AuthorizationMicroservice.Application.CryptographyService;
using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Application.Infrastructure;
using AuthorizationMicroservice.Application.Infrastructure.Exceptions;
using AuthorizationMicroservice.Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Org.BouncyCastle.Ocsp;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Application.UsersService
{
    public class Register
    {
        public class Command : IRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public UserInfoDto UserInfo { get; set; } = new UserInfoDto();
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Email).EmailAddress().NotNull().Length(3, 50);
                RuleFor(x => x.Password).Length(3, 25).NotNull().NotEmpty();
                RuleFor(x => x.UserInfo.Firstname).Length(1, 25).NotEmpty().NotNull();
                RuleFor(x => x.UserInfo.Lastname).Length(1, 50).NotEmpty().NotNull();
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
                    string salt = SaltHandler.Create();
                    string hashedPassword = HashHandler.Create(request.Password, salt);

                    UserInfo userInfo = unitOfWork.UserInfoMap.MapUserInfo(request.UserInfo);
                    userInfo.RegistrationTime = new LocalDateTime().LocalDate;


                    UserCredential user = new UserCredential
                    {
                        Email = request.Email,
                        PasswordSalt = salt,
                        PasswordHash = hashedPassword,
                        UserInfo = userInfo
                    };


                    await unitOfWork.UserRepository.InsertRecordAsync(user);
                    unitOfWork.Logger.LogInformation($"User was created with id: {user.Id}");
                    return Unit.Value;
                }
                catch (MongoWriteException)
                {
                    unitOfWork.Logger.LogInformation($"User { request.Email } was not found");

                    throw new StatusCodeException(HttpStatusCode.Conflict, "User with this email already exists");
                }


            }
        }
    }

}
