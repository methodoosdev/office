using App.Data;
using App.Models.Traders;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class WorkingAreaValidator : BaseNopValidator<WorkingAreaModel>
    {
        public WorkingAreaValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.WorkingAreaModel.Validation.Description"));            
        }
    }
}