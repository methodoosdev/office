using App.Data;
using App.Models.SimpleTask;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class SimpleTaskCategoryValidator : BaseNopValidator<SimpleTaskCategoryModel>
    {
        public SimpleTaskCategoryValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.SimpleTaskCategoryModel.Validation.Description"));
        }
    }
}