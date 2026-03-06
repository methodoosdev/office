using App.Data;
using App.Models.Customers;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class CustomerPermissionValidator : BaseNopValidator<CustomerPermissionModel>
    {
        public CustomerPermissionValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.CustomerPermissionModel.Validation.Name"));
            RuleFor(x => x.SystemName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.CustomerPermissionModel.Validation.SystemName"));
        }
    }
}