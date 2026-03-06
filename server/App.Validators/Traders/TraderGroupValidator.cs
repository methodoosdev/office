using App.Data;
using App.Models.Traders;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class TraderGroupValidator : BaseNopValidator<TraderGroupModel>
    {
        public TraderGroupValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderGroupModel.Validation.Description"));
        }
    }
}