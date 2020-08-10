using AutoMapper;
using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Domain.Models;
using System.Collections.Generic;

namespace AuthorizationMicroservice.Application.Infrastructure.Mapper
{
    public class UserMap : IUserMap
    {
        private readonly IMapper mapper;

        public UserMap()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserCredential, UserCredentialDto>()
                             .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                             .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                             .ForMember(dst => dst.AccessToken, opt => opt.MapFrom(src => src.AccessToken))
                             .ForMember(dst => dst.UserInfo, opt => opt.MapFrom(src => src.UserInfo));

            });
            mapper = config.CreateMapper();
        }

        public UserCredentialDto MapUserDto(UserCredential user)
        {
            return mapper.Map<UserCredential, UserCredentialDto>(user);
        }

        public List<UserCredentialDto> MapUserDtoList(List<UserCredential> users)
        {
            return mapper.Map<List<UserCredential>, List<UserCredentialDto>>(users);
        }

        public UserCredential MapUserCredential(UserCredentialDto userDto)
        {
            return mapper.Map<UserCredentialDto, UserCredential>(userDto);
        }
    }
}
