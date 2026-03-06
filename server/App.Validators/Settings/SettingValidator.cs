using App.Core.Domain.Configuration;
using App.Data;
using App.Models.Settings;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class SettingValidator : BaseNopValidator<SettingModel>
    {
        public SettingValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.Name.Required"));

            SetDatabaseValidationRules<Setting>(dataProvider);
        }
    }
}