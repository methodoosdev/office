using App.Data;
using App.Models.SimpleTask;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class SimpleTaskManagerValidator : BaseNopValidator<SimpleTaskManagerModel>
    {
        public SimpleTaskManagerValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.AssignorId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.Validation.AssignorId"));
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.Validation.Name"));
            RuleFor(x => x.StartingDate).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.Validation.StartingDate"));
            RuleFor(x => x.EndingDate).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.Validation.EndingDate"));
            RuleFor(x => x.CreatedDate).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.Validation.CreatedDate"));
        }
    }
}