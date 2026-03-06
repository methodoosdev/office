using App.Data;
using App.Models.Payroll;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class WorkerScheduleShiftValidator : BaseNopValidator<WorkerScheduleShiftModel>
    {
        public WorkerScheduleShiftValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.TraderId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.WorkerScheduleShiftModel.Validation.TraderId"));
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.WorkerScheduleShiftModel.Validation.Description"));

            //RuleFor(x => x.NonstopToDate).Must((x, nonstopToDate) =>
            //{
            //    var value = nonstopToDate.AddHours(2).TimeOfDay.TotalHours;
            //    return value > 0;
            //}).WithMessageAwait(localizationService.GetResourceAsync("App.Models.CustomerModel.Validation.NonstopToDate"));
        }
    }
}