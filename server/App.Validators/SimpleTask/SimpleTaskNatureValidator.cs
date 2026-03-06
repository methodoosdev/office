using App.Data;
using App.Models.SimpleTask;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class SimpleTaskNatureValidator : BaseNopValidator<SimpleTaskNatureModel>
    {
        public SimpleTaskNatureValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.SimpleTaskNatureModel.Validation.Description"));
        }
    }
}