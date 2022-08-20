using FluentValidation;
using Application.DataTransferObjects;

namespace Application.Validations
{
    public class UserValidator : AbstractValidator<UserCreateDTO>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be null or empty");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Enter a Valid Email Address").When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName cannot be null or empty");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName cannot be null or empty");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone Number cannot be null or empty");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password be null or empty");
            RuleFor(x => x.Password).Matches(@"(?-i)(?=^.{8,}$)((?!.*\s)(?=.*[A-Z])(?=.*[a-z]))((?=(.*\d){1,})|(?=(.*\W){1,}))^.*$")
                .WithMessage("Password must be at least 8 characters, at least 1 upper case letters (A – Z), Atleast 1 lower case letters (a – z), Atleast 1 number (0 – 9) or non-alphanumeric symbol (e.g. @ '$%£! ')");
        }
    }
    public class UserLoginValidator : AbstractValidator<UserLoginDTO>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be null or empty");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Enter a Valid Email Address").When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be null or empty");
        }
    }
    public class ResetPassowordValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPassowordValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be null or empty");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Enter a Valid Email Address").When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }

    //public class SetPasswordValidator : AbstractValidator<SetPasswordDTO>
    //{
    //    public SetPasswordValidator()
    //    {
    //        RuleFor(x => x.Token).NotEmpty().WithMessage("Token cannot be null or empty");
    //        RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be null or empty");
    //        RuleFor(x => x.Password).Matches(@"(?-i)(?=^.{8,}$)((?!.*\s)(?=.*[A-Z])(?=.*[a-z]))((?=(.*\d){1,})|(?=(.*\W){1,}))^.*$")
    //            .WithMessage("Password must be at least 8 characters, at least 1 upper case letters (A – Z), Atleast 1 lower case letters (a – z), Atleast 1 number (0 – 9) or non-alphanumeric symbol (e.g. @ '$%£! ')");
    //    }
    //}
}
