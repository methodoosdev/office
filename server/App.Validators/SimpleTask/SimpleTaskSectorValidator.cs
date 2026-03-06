using App.Data;
using App.Models.SimpleTask;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class SimpleTaskSectorValidator : BaseNopValidator<SimpleTaskSectorModel>
    {
        public SimpleTaskSectorValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.SimpleTaskSectorModel.Validation.Description"));
        }
    }
}