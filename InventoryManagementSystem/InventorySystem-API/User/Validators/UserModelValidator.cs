using FluentValidation;
using InventorySystem_API.User.Model;

namespace InventorySystem_API.User.Validator
{
    public class UserModelValidator : AbstractValidator<UserModel>
    {
        public UserModelValidator()
        {

            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");


        }
    }
}
