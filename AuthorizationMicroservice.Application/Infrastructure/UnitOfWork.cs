using AuthorizationMicroservice.Application.CryptographyService;
using AuthorizationMicroservice.Application.Infrastructure.Exceptions;
using AuthorizationMicroservice.Application.Infrastructure.Mapper;
using AuthorizationMicroservice.Persistance.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthorizationMicroservice.Application.Infrastructure
{
    public class UnitOfWork
    {
        private readonly IUserRepository userRepository;
        private readonly IMediator mediator;
        private readonly IUserMap userMap;
        private readonly IUserInfoMap userInfoMap;
        private readonly IJWTHandler jwtHandler;
        private readonly ILogger<ApplicationLogger> logger;
        private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;
        private readonly IAesHandler aesHandler;


        public UnitOfWork(
            IUserRepository userRepository,
            IMediator mediator,
            IUserMap userMap,
            IJWTHandler jwtHandler,
            IUserInfoMap userInfoMap,
            ILogger<ApplicationLogger> logger,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IAesHandler aesHandler
            )
        {
            this.userRepository = userRepository;
            this.mediator = mediator;
            this.userMap = userMap;
            this.jwtHandler = jwtHandler;
            this.userInfoMap = userInfoMap;
            this.logger = logger;
            this.configuration = configuration;
            this.aesHandler = aesHandler;
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (this.userRepository == null) throw new NullImplementationException("User Repository does not exist!");

                return userRepository;
            }
        }

        public IMediator Mediator
        {
            get
            {
                if (this.mediator == null) throw new NullImplementationException("Mediator does not exist!");

                return mediator;
            }
        }

        public IUserMap UserMap
        {
            get
            {
                if (this.UserMap == null) throw new NullImplementationException("User map does not exist!");

                return userMap;
            }
        }

        public IUserInfoMap UserInfoMap
        {
            get
            {
                if (this.userInfoMap == null) throw new NullImplementationException("User info map does not exist!");

                return userInfoMap;
            }
        }

        public IJWTHandler JWTHandler
        {
            get
            {
                if (this.jwtHandler == null) throw new NullImplementationException("JWT Handler does not exist!");

                return jwtHandler;
            }
        }

        public ILogger<ApplicationLogger> Logger
        {
            get
            {
                if (this.logger == null) throw new NullImplementationException("Logger does not exist!");

                return logger;
            }
        }

        public Microsoft.Extensions.Configuration.IConfiguration Configuration
        {
            get
            {
                if (this.configuration == null) throw new NullImplementationException("Configuration does not exist!");

                return configuration;
            }
        }
        public IAesHandler AesHandler
        {
            get
            {
                if (this.aesHandler == null) throw new NullImplementationException("AES handler does not exist!");

                return aesHandler;
            }
        }

    }
}
