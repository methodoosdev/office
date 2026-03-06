using App.Data;
using App.Models.Payroll;
using App.Services.Localization;
using FluentValidation;
using System;

namespace App.Validators
{
    public partial class WorkerScheduleValidator : BaseNopValidator<WorkerScheduleModel>
    {
        public WorkerScheduleValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.TraderId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.WorkerScheduleModel.Validation.TraderId"));
            RuleFor(x => x.Workers).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.WorkerScheduleModel.Validation.Workers"));

            RuleFor(x => x.PeriodFromDate).Must((x, periodFromDate) =>
            {
                var date = DateTime.UtcNow;
                return (periodFromDate < new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0)) ? false : true;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.WorkerScheduleModel.Validation.PeriodFromDate"));

            RuleFor(x => x.PeriodToDate).Must((x, periodToDate) =>
            {
                return (periodToDate < x.PeriodFromDate) ? false : true;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.WorkerScheduleModel.Validation.PeriodToDate"));

        }
    }
}