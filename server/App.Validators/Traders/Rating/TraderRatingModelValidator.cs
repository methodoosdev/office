using App.Data;
using App.Models.Traders;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class TraderRatingModelValidator : BaseNopValidator<TraderRatingModel>
    {
        public TraderRatingModelValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderRatingModel.Validation.Description"));

            RuleFor(x => x.TraderRatingCategoryId).GreaterThan(0).
                WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderRatingModel.Validation.TraderRatingCategoryId"));
            RuleFor(x => x.DepartmentId).GreaterThan(0).
                WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderRatingModel.Validation.DepartmentId"));

        }
    }
}