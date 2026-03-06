using App.Core.Domain.Localization;
using App.Data;
using App.Models.Localization;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class LanguageResourceValidator : BaseNopValidator<LocaleResourceModel>
    {
        public LanguageResourceValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            //if validation without this set rule is applied, in this case nothing will be validated
            //it's used to prevent auto-validation of child models
            RuleSet(NopValidationDefaults.ValidationRuleSet, () =>
            {
                RuleFor(model => model.ResourceName)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.Fields.Name.Required"));

                RuleFor(model => model.ResourceValue)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.Fields.Value.Required"));

                SetDatabaseValidationRules<LocaleStringResource>(dataProvider);
            });
        }
    }
}