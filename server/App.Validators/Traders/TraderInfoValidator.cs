using App.Data;
using App.Models.Traders;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class TraderInfoValidator : BaseNopValidator<TraderInfoModel>
    {
        public TraderInfoValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.SortDescription).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderInfoModel.Validation.SortDescription"));
        }
    }
}