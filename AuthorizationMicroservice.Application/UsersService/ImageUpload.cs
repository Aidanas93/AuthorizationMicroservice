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
    public class ImageUpload
    {
        public class Command : IRequest<AccessTokenDto>
        {
            public Guid Id { get; set; }
            public string JwtToken { get; set; }
            public string PictureUrl { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.PictureUrl).NotNull();
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

                var token = unitOfWork.JWTHandler.RemoveBearerFromJWTToken(request.JwtToken);
                var tokenId = unitOfWork.JWTHandler.TokenId(token);
                if (tokenId == request.Id)
                {
                    var user = await unitOfWork.UserRepository.LoadRecordByIdAsync(request.Id);

                    user.UserInfo.PictureUrl = request.PictureUrl;
                    await unitOfWork.UserRepository.UpsertRecordAsync(request.Id, user);

                    unitOfWork.Logger.LogInformation($"User ID:{ user.Id } Status code: { HttpStatusCode.OK }");
                    return new AccessTokenDto() { Id = user.Id, Token = user.AccessToken.JWTToken };
                }
                else
                {
                    throw new StatusCodeException(HttpStatusCode.Unauthorized, $"User was not authorized", request.Id);
                }

            }
        }
    }
}
