using App.Data;
using App.Models.Traders;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class TraderRatingCategoryModelValidator : BaseNopValidator<TraderRatingCategoryModel>
    {
        public TraderRatingCategoryModelValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderRatingCategoryModel.Validation.Description"));
        }
    }
}