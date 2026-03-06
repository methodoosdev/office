using App.Data;
using App.Models.Employees;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class SpecialtyValidator : BaseNopValidator<SpecialtyModel>
    {
        public SpecialtyValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.SpecialtyModel.Validation.Description"));
        }
    }
}