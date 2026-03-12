using FluentValidation;
using InventorySystem_Shared.Company;
using MongoDB.Driver;

namespace InventorySystem_API.Company.Validation
{
    public class CompanyValidator : AbstractValidator<CompanyDTO>
    {
        public CompanyValidator() 
        {
            RuleFor(company => company.Name)
                .NotEmpty().WithMessage("Назва компанії - обов'язкове поле");

            RuleFor(company => company.Description)
                .MaximumLength(250).WithMessage("Опис компанії до 250 символів")
                .When(company => company.Description is not null);

            RuleFor(company => company.Phone)
                .Matches(@"^\+380\d{9}$").WithMessage("Номер телефона має відповідати формату: '+380xxxxxxxxxx'");
                
        }
    }
}
