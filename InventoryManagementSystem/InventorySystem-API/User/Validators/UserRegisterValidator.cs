using FluentValidation;
using InventorySystem_Shared.User;

namespace InventorySystem_API.User.Validator
{
    public class UserRegisterValidator : AbstractValidator<UserRegister>
    {
        public UserRegisterValidator() 
        {
            RuleFor(user => user.Name)
                 .NotEmpty().WithMessage("Ім'я обов'язкове пр реєстрації")
                 .MaximumLength(100).WithMessage("Ім'я не може перевищувати 100 символів.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Пошта обов'язкова.")
                .EmailAddress().WithMessage("Невірний формат електроної адреси.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Пароль обов'язковий.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_])[A-Za-z\d@$!%*?&_]{8,}$")
                .WithMessage("Парль має містити принаймні одну велику та малу англійську літеру, спеціальний символ та мати довжину мінімум вісім символів");

            RuleFor(user => user.CompanyId)
                .NotEmpty().WithMessage("Ідентифікатор для компанії обов'язковий")
                .Length(24).WithMessage("Ідентифікатор має відповідає формату BSON.Id");

            RuleFor(user => user.UserRole)
                .IsInEnum().WithMessage("Роль має входити в перелік");
        }
    }
}
