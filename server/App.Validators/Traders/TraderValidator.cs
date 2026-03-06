using App.Core.Infrastructure;
using App.Data;
using App.Models.Traders;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class TraderValidator : BaseNopValidator<TraderModel>
    {
        public TraderValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Vat).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderModel.Validation.Vat"));
            RuleFor(x => x.LastName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderModel.Validation.LastName"));

            RuleFor(x => x.Email).Must((x, context) =>
            {
                return !string.IsNullOrEmpty(x.Email) ? CommonHelper.IsValidEmail(x.Email) : true;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderModel.Validation.Email"));

            RuleFor(x => x.Email2).Must((x, context) =>
            {
                return !string.IsNullOrEmpty(x.Email2) ? CommonHelper.IsValidEmail(x.Email2) : true;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderModel.Validation.Email2"));

            RuleFor(x => x.Email3).Must((x, context) =>
            {
                return !string.IsNullOrEmpty(x.Email3) ? CommonHelper.IsValidEmail(x.Email3) : true;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderModel.Validation.Email3"));
        }
    }
}