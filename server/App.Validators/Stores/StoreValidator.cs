using App.Core.Domain.Stores;
using App.Data;
using App.Services.Localization;
using FluentValidation;
using App.Models.Stores;

namespace App.Validators
{
    public partial class StoreValidator : BaseNopValidator<StoreModel>
    {
        public StoreValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Stores.Fields.Name.Required"));
            RuleFor(x => x.Url).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Stores.Fields.Url.Required"));

            SetDatabaseValidationRules<Store>(dataProvider);
        }
    }
}