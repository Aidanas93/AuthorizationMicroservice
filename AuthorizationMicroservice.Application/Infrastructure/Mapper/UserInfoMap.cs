using AutoMapper;
using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Domain.Models;

namespace AuthorizationMicroservice.Application.Infrastructure.Mapper
{
    public class UserInfoMap : IUserInfoMap
    {
        private readonly IMapper mapper;

        public UserInfoMap()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.CreateMap<UserInfoDto, UserInfo>()
                            .ForMember(dst => dst.Firstname, opt => opt.MapFrom(src => src.Firstname))
                            .ForMember(dst => dst.Lastname, opt => opt.MapFrom(src => src.Lastname))
                            .ForMember(dst => dst.PictureUrl, opt => opt.MapFrom(src => src.PictureUrl));
            });

            mapper = mappingConfig.CreateMapper();
        }
        public UserInfo MapUserInfo(UserInfoDto userInfo)
        {
            return mapper.Map<UserInfo>(userInfo);
        }
    }
}
