using App.Data;
using App.Models.Assignment;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class AssignmentPrototypeValidator : BaseNopValidator<AssignmentPrototypeModel>
    {
        public AssignmentPrototypeValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentPrototypeModel.Validation.Name"));
        }
    }
}