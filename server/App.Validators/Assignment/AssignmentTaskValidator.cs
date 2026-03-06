using App.Data;
using App.Models.Assignment;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class AssignmentTaskValidator : BaseNopValidator<AssignmentTaskModel>
    {
        public AssignmentTaskValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.AssignmentPrototypeId).Must((x, context) =>
            {
                if (x.Id > 0)
                    return true;
                return x.AssignmentPrototypeId > 0;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentTaskModel.Validation.AssignmentPrototypeId"));

            RuleFor(x => x.TraderId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentTaskModel.Validation.TraderId"));

            RuleFor(x => x.AssignorId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentTaskModel.Validation.AssignorId"));

            RuleFor(x => x.Name).Must((x, context) =>
            {
                if (x.Id > 0)
                    return !string.IsNullOrEmpty(x.Name) && !string.IsNullOrWhiteSpace(x.Name);
                return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.AssignmentTaskModel.Validation.Name"));
        }
    }
}