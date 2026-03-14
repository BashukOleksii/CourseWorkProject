using FluentValidation;
using InventorySystem_Shared.Warehouse;

namespace InventorySystem_API.Warehouse.Validation
{
    public class WarehouseValidator : AbstractValidator<WarehouseDTO>
    {
        public WarehouseValidator() 
        {
            RuleFor(warehouse => warehouse.Name)
                .NotEmpty().WithMessage("Назва калду обов'язкова")
                .MaximumLength(50).WithMessage("Максимальна довжина назви складу 50 символів");

            RuleFor(warehouse => warehouse.Description)
                .MaximumLength(255).WithMessage("Максимальна довжина 255 символів для опису складу")
                .When(warehouse => warehouse.Description is not null);

            RuleFor(warehouse => warehouse.Area)
                .NotNull().WithMessage("Площа складу це обов'язкове поле")
                .GreaterThan(0).WithMessage("Площа складумає бути більшою за 0");
                
        }
    }
}
