using App.Data;
using App.Models.Assignment;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class AssignmentPrototypeActionValidator : BaseNopValidator<AssignmentPrototypeActionModel>
    {
        public AssignmentPrototypeActionValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentPrototypeActionModel.Validation.DepartmentId"));

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentPrototypeActionModel.Validation.Name"));
        }
    }
}