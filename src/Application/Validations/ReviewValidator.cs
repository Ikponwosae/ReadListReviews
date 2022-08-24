using FluentValidation;
using Application.DataTransferObjects;

namespace Application.Validations
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDTO>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be null or empty").MaximumLength(370);         
        }
    }
    
}
