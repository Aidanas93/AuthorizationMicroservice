using FluentValidation;
using AuthorizationMicroservice.Application.Dto;

namespace AuthorizationMicroservice.Application.Infrastructure.Validation
{
    public class UserCredentialsValidator : AbstractValidator<UserCredentialDto>
    {
        public UserCredentialsValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Email).EmailAddress().NotNull().Length(3, 50);
            RuleFor(x => x.UserInfo.Firstname).Length(1, 25).NotEmpty().NotNull();
            RuleFor(x => x.UserInfo.Lastname).Length(1, 50).NotEmpty().NotNull();
            RuleFor(x => x.UserInfo.RegistrationTime).NotNull().NotEmpty();
        }
    }
}
