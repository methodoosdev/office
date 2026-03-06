using App.Data;
using App.Models.Assignment;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class AssignmentReasonValidator : BaseNopValidator<AssignmentReasonModel>
    {
        public AssignmentReasonValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentReasonModel.Validation.Description"));
        }
    }
}