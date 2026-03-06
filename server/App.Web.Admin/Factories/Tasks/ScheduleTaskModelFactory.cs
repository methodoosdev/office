using App.Core.Domain.ScheduleTasks;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models;
using App.Framework.Models.Extensions;
using App.Models.Tasks;
using App.Services.Common;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Admin.Factories
{
    public partial interface IScheduleTaskModelFactory
    {
        Task<ScheduleTaskSearchModel> PrepareScheduleTaskSearchModelAsync(ScheduleTaskSearchModel searchModel);
        Task<ScheduleTaskListModel> PrepareScheduleTaskListModelAsync(ScheduleTaskSearchModel searchModel);
        Task<ScheduleTaskModel> PrepareScheduleTaskModelAsync(ScheduleTaskModel model, ScheduleTask scheduleTask);
        Task<ScheduleTaskFormModel> PrepareScheduleTaskFormModelAsync(ScheduleTaskFormModel formModel);
    }
    public partial class ScheduleTaskModelFactory : IScheduleTaskModelFactory
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IScheduleTaskService _scheduleTaskService;

        public ScheduleTaskModelFactory(IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IScheduleTaskService scheduleTaskService)
        {
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
        }

        public virtual async Task<ScheduleTaskSearchModel> PrepareScheduleTaskSearchModelAsync(ScheduleTaskSearchModel searchModel)
        {
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScheduleTaskModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScheduleTaskListModel> PrepareScheduleTaskListModelAsync(ScheduleTaskSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get schedule tasks
            var scheduleTasks = (await _scheduleTaskService.GetAllTasksAsync(true))
                .Where(task => !string.Equals(task.Name, nameof(ResetLicenseCheckTask), StringComparison.InvariantCultureIgnoreCase))
                .ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = await new ScheduleTaskListModel().PrepareToGridAsync(searchModel, scheduleTasks, () =>
            {
                return scheduleTasks.SelectAwait(async scheduleTask =>
                {
                    //fill in model values from the entity
                    var scheduleTaskModel = scheduleTask.ToModel<ScheduleTaskModel>();

                    //convert dates to the user time
                    if (scheduleTask.LastStartUtc.HasValue)
                    {
                        scheduleTaskModel.LastStartUtc = (await _dateTimeHelper
                            .ConvertToUserTimeAsync(scheduleTask.LastStartUtc.Value, DateTimeKind.Utc)).ToString("G");
                    }

                    if (scheduleTask.LastEndUtc.HasValue)
                    {
                        scheduleTaskModel.LastEndUtc = (await _dateTimeHelper
                            .ConvertToUserTimeAsync(scheduleTask.LastEndUtc.Value, DateTimeKind.Utc)).ToString("G");
                    }

                    if (scheduleTask.LastSuccessUtc.HasValue)
                    {
                        scheduleTaskModel.LastSuccessUtc = (await _dateTimeHelper
                            .ConvertToUserTimeAsync(scheduleTask.LastSuccessUtc.Value, DateTimeKind.Utc)).ToString("G");
                    }

                    return scheduleTaskModel;
                });
            });
            return model;
        }

        public virtual async Task<ScheduleTaskModel> PrepareScheduleTaskModelAsync(ScheduleTaskModel model, ScheduleTask scheduleTask)
        {
            //fill in model values from the entity
            if (scheduleTask != null)
            {
                model ??= scheduleTask.ToModel<ScheduleTaskModel>();
                model.LastStartUtc = scheduleTask.LastStartUtc.HasValue ? (await _dateTimeHelper
                            .ConvertToUserTimeAsync(scheduleTask.LastStartUtc.Value, DateTimeKind.Utc)).ToString("G") : null;
                model.LastEndUtc = scheduleTask.LastEndUtc.HasValue ? (await _dateTimeHelper
                            .ConvertToUserTimeAsync(scheduleTask.LastEndUtc.Value, DateTimeKind.Utc)).ToString("G") : null;
                model.LastSuccessUtc = scheduleTask.LastSuccessUtc.HasValue ? (await _dateTimeHelper
                            .ConvertToUserTimeAsync(scheduleTask.LastSuccessUtc.Value, DateTimeKind.Utc)).ToString("G") : null;
            }

            return model;
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScheduleTaskModel>(1, nameof(ScheduleTaskModel.Name), ColumnType.RouterLink),
                ColumnConfig.Create<ScheduleTaskModel>(2, nameof(ScheduleTaskModel.Seconds)),
                ColumnConfig.Create<ScheduleTaskModel>(3, nameof(ScheduleTaskModel.Enabled), ColumnType.Checkbox),
                ColumnConfig.Create<ScheduleTaskModel>(4, nameof(ScheduleTaskModel.StopOnError), ColumnType.Checkbox),
                ColumnConfig.Create<ScheduleTaskModel>(5, nameof(ScheduleTaskModel.LastStartUtc), ColumnType.Text),
                ColumnConfig.Create<ScheduleTaskModel>(6, nameof(ScheduleTaskModel.LastEndUtc), ColumnType.Text),
                ColumnConfig.Create<ScheduleTaskModel>(7, nameof(ScheduleTaskModel.LastSuccessUtc), ColumnType.Text),
                ColumnConfig.CreateButton<ScheduleTaskModel>(0, ColumnType.RowButton, "runNow", "info",
                    await _localizationService.GetResourceAsync("App.Common.RunNow"), textAlign, textAlign)
            };

            return columns;
        }

        public virtual async Task<ScheduleTaskFormModel> PrepareScheduleTaskFormModelAsync(ScheduleTaskFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScheduleTaskModel>(nameof(ScheduleTaskModel.Name), FieldType.Text, _readonly: true),
                FieldConfig.Create<ScheduleTaskModel>(nameof(ScheduleTaskModel.Seconds), FieldType.Numeric),
                FieldConfig.Create<ScheduleTaskModel>(nameof(ScheduleTaskModel.Enabled), FieldType.Checkbox),
                FieldConfig.Create<ScheduleTaskModel>(nameof(ScheduleTaskModel.StopOnError), FieldType.Checkbox),
                FieldConfig.Create<ScheduleTaskModel>(nameof(ScheduleTaskModel.LastStartUtc), FieldType.Text, _readonly: true),
                FieldConfig.Create<ScheduleTaskModel>(nameof(ScheduleTaskModel.LastEndUtc), FieldType.Text, _readonly: true),
                FieldConfig.Create<ScheduleTaskModel>(nameof(ScheduleTaskModel.LastSuccessUtc), FieldType.Text, _readonly: true)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScheduleTaskModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}