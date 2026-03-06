using App.Data;
using App.Models.Payroll;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators.Payroll
{
    public partial class WorkerLeaveDetailValidator : BaseNopValidator<WorkerLeaveDetailModel>
    {
        public WorkerLeaveDetailValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.TraderId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.WorkerLeaveDetailModel.Validation.TraderId"));
            RuleFor(x => x.WorkerId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.WorkerLeaveDetailModel.Validation.WorkerId"));
        }
    }
}
