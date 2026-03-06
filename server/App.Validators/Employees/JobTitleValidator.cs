using App.Data;
using App.Models.Employees;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class JobTitleValidator : BaseNopValidator<JobTitleModel>
    {
        public JobTitleValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.JobTitleModel.Validation.Description"));
        }
    }
}