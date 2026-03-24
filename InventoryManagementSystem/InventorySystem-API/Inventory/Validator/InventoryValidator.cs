using FluentValidation;
using InventorySystem_Shared.Inventory;

namespace InventorySystem_API.Inventory.Validator
{
    public class InventoryValidator : AbstractValidator<InventoryCreate>
    {
        public InventoryValidator() 
        {
            RuleFor(inventory => inventory.Name)
                .NotEmpty().WithMessage("Назва товару обов'язкова");
            RuleFor(inventory => inventory.Description)
                .NotEmpty().WithMessage("Опис товару обов'язковий")
                .MinimumLength(5).WithMessage("Мінімальна довжина опису 5 символів");
            RuleFor(inventory => inventory.Price)
                .GreaterThan(0).WithMessage("Ціна товару має бути більша за нуль");
            RuleFor(inventory => inventory.InventoryType)
                .IsInEnum().WithMessage("Категорія товару має бути в переліку категорій")



        }
    }
}
