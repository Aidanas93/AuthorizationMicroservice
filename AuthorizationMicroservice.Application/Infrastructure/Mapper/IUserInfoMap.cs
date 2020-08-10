using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Domain.Models;

namespace AuthorizationMicroservice.Application.Infrastructure.Mapper
{
    public interface IUserInfoMap
    {
        UserInfo MapUserInfo(UserInfoDto userInfo);
    }
}
