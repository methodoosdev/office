using App.Core.Domain.Directory;
using App.Data;
using App.Models.Directory;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class CountryValidator : BaseNopValidator<CountryModel>
    {
        public CountryValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.Name.Required"));
            RuleFor(p => p.Name).Length(1, 100);

            RuleFor(x => x.TwoLetterIsoCode)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.TwoLetterIsoCode.Required"));
            RuleFor(x => x.TwoLetterIsoCode)
                .Length(2)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.TwoLetterIsoCode.Length"));

            RuleFor(x => x.ThreeLetterIsoCode)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode.Required"));
            RuleFor(x => x.ThreeLetterIsoCode)
                .Length(3)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode.Length"));

            SetDatabaseValidationRules<Country>(dataProvider);
        }
    }
}