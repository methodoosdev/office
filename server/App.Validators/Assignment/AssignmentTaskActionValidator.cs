using App.Data;
using App.Models.Assignment;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class AssignmentTaskActionValidator : BaseNopValidator<AssignmentTaskActionModel>
    {
        public AssignmentTaskActionValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.AssignmentTaskId).GreaterThan(0).
                WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentTaskActionModel.Validation.AssignmentTaskId"));
            RuleFor(x => x.EmployeeId).GreaterThan(0).
                WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentTaskActionModel.Validation.EmployeeId"));

            RuleFor(x => x.ActionName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentTaskActionModel.Validation.ActionName"));
        }
    }
}