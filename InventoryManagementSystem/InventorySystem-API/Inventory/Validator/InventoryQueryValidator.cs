using FluentValidation;
using InventorySystem_Shared.Inventory;

namespace InventorySystem_API.Inventory.Validator
{
    public class InventoryQueryValidator : AbstractValidator<InventoryQuery>
    {
        public InventoryQueryValidator() 
        {
            RuleFor(inventory => inventory.Page)
                .GreaterThan(0).WithMessage("Номер сторінк має бути додатній");

            RuleFor(inventory => inventory.PageSize)
                .GreaterThan(0).WithMessage("Кількість записів має бути додатня");

            RuleFor(inventory => inventory.MinPrice)
                .LessThan(inventory => inventory.MaxPrice)
                .When(inventory => inventory.MaxPrice.HasValue)
                .WithMessage("Ліва межа ціни має бути меншою за праву");

            RuleFor(inventory => inventory.MinQuantity)
                .LessThan(inventory => inventory.MaxQuantity)
                .When(inventory => inventory.MaxQuantity.HasValue)
                .WithMessage("Ліва межа квлькості має бути меншою за праву");

        }
    }
}
