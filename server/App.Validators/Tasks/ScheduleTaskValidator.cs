using App.Core.Domain.ScheduleTasks;
using App.Data;
using App.Models.Tasks;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class ScheduleTaskValidator : BaseNopValidator<ScheduleTaskModel>
    {
        public ScheduleTaskValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.ScheduleTasks.Name.Required"));
            RuleFor(x => x.Seconds).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("Admin.System.ScheduleTasks.Seconds.Positive"));

            SetDatabaseValidationRules<ScheduleTask>(dataProvider);
        }
    }
}