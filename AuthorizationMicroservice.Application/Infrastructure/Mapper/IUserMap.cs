using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Domain.Models;
using System.Collections.Generic;

namespace AuthorizationMicroservice.Application.Infrastructure.Mapper
{
    public interface IUserMap
    {
        UserCredentialDto MapUserDto(UserCredential user);
        List<UserCredentialDto> MapUserDtoList(List<UserCredential> users);
        UserCredential MapUserCredential(UserCredentialDto userDto);
    }
}
