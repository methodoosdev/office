using App.Data;
using App.Models.Employees;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class EducationValidator : BaseNopValidator<EducationModel>
    {
        public EducationValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.EducationModel.Validation.Description"));
        }
    }
}