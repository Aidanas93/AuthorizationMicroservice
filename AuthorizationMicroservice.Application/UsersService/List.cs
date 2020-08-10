using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Application.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Application.UsersService
{
    public class List
    {
        public class Query : IRequest<List<UserCredentialDto>> { }

        public class Handler : IRequestHandler<Query, List<UserCredentialDto>>
        {
            private readonly UnitOfWork unitOfWork;

            public Handler(UnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<List<UserCredentialDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var users = await unitOfWork.UserRepository.LoadRecordsAsync();

                unitOfWork.Logger.LogInformation($"Get User List with count: {users.Count}, HTTP 200");


                List<UserCredentialDto> userCredentialDtos = new List<UserCredentialDto>();

                users.ForEach(user => userCredentialDtos.Add(
                    new UserCredentialDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserInfo = user.UserInfo,
                        AccessToken = user.AccessToken
                    }));

                return userCredentialDtos;
            }
        }
    }
}
