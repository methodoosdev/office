using App.Data;
using App.Models.Customers;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class CustomerRoleValidator : BaseNopValidator<CustomerRoleModel>
    {
        public CustomerRoleValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.CustomerRoleModel.Validation.Name"));
        }
    }
}