using App.Core.Domain.Localization;
using App.Data;
using App.Models.Localization;
using App.Services.Localization;
using FluentValidation;
using System.Globalization;

namespace App.Validators
{
    public partial class LanguageValidator : BaseNopValidator<LanguageModel>
    {
        public LanguageValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.LanguageModel.Validation.Name"));
            RuleFor(x => x.LanguageCulture)
                .Must(x =>
                {
                    try
                    {
                        //let's try to create a CultureInfo object
                        //if "DisplayLocale" is wrong, then exception will be thrown
                        var unused = new CultureInfo(x);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                })
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.LanguageModel.Validation.LanguageCulture"));

            RuleFor(x => x.UniqueSeoCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.LanguageModel.Validation.UniqueSeoCode.NotEmpty"));
            RuleFor(x => x.UniqueSeoCode).Length(2).WithMessageAwait(localizationService.GetResourceAsync("App.Models.LanguageModel.Validation.UniqueSeoCode.Length"));

            SetDatabaseValidationRules<Language>(dataProvider, "UniqueSeoCode");
        }
    }
}