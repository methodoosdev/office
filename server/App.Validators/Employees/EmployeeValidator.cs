using App.Data;
using App.Models.Employees;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class EmployeeValidator : BaseNopValidator<EmployeeModel>
    {
        public EmployeeValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.EmployeeModel.Validation.FirstName"));
            RuleFor(x => x.LastName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.EmployeeModel.Validation.LastName"));
            RuleFor(x => x.EmailContact).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.EmployeeModel.Validation.EmailContact"));
        }
    }
}