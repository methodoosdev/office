using App.Core.Domain.Directory;
using App.Data;
using App.Models.Directory;
using App.Services.Localization;
using FluentValidation;
using System.Globalization;

namespace App.Validators
{
    public partial class CurrencyValidator : BaseNopValidator<CurrencyModel>
    {
        public CurrencyValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.CurrencyModel.Validation.Name.NotEmpty"))
                .Length(1, 50).WithMessageAwait(localizationService.GetResourceAsync("App.Models.CurrencyModel.Validation.Name.Length"));
            RuleFor(x => x.CurrencyCode)
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.CurrencyModel.Validation.CurrencyCode.NotEmpty"))
                .Length(1, 5).WithMessageAwait(localizationService.GetResourceAsync("App.Models.CurrencyModel.Validation.CurrencyCode.Length"));
            RuleFor(x => x.Rate)
                .GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.CurrencyModel.Validation.Rate"));
            RuleFor(x => x.CustomFormatting)
                .Length(0, 50).WithMessageAwait(localizationService.GetResourceAsync("App.Models.CurrencyModel.Validation.CustomFormatting"));
            RuleFor(x => x.DisplayLocale)
                .Must(x =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(x))
                            return true;
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
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.CurrencyModel.Validation.DisplayLocale"));

            SetDatabaseValidationRules<Currency>(dataProvider);
        }
    }
}