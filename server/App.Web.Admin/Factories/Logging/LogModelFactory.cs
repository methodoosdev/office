using App.Core;
using App.Core.Domain.Logging;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Logging;
using App.Services;
using App.Services.Customers;
using App.Services.Helpers;
using App.Services.Html;
using App.Services.Localization;
using App.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Admin.Factories.Logging
{
    public partial interface ILogModelFactory
    {
        Task<LogSearchModel> PrepareLogSearchModelAsync(LogSearchModel searchModel);
        Task<LogListModel> PrepareLogListModelAsync(LogSearchModel searchModel);
        Task<LogModel> PrepareLogModelAsync(LogModel model, Log log, bool excludeProperties = false);
        Task<LogFormModel> PrepareLogFormModelAsync(LogFormModel formModel);
    }
    public partial class LogModelFactory : ILogModelFactory
    {
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        public LogModelFactory(
            IModelFactoryService modelFactoryService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            ILogger logger)
        {
            _modelFactoryService = modelFactoryService;
            _dateTimeHelper = dateTimeHelper;
            _customerService = customerService;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _logger = logger;
        }

        public virtual async Task<LogSearchModel> PrepareLogSearchModelAsync(LogSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.LogModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        private async Task<IPagedList<LogModel>> GetPagedListAsync(LogSearchModel searchModel)
        {
            var query = _logger.Table
                .SelectAwait(async logItem =>
                {
                    var logModel = logItem.ToModel<LogModel>();

                    //convert dates to the user time
                    logModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);
                    //logModel.CreatedOn = new DateTime(logItem.CreatedOnUtc.Year, logItem.CreatedOnUtc.Month, logItem.CreatedOnUtc.Day, logItem.CreatedOnUtc.Hour, logItem.CreatedOnUtc.Minute, logItem.CreatedOnUtc.Second, logItem.CreatedOnUtc.Millisecond, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    logModel.LogLevelName = await _localizationService.GetLocalizedEnumAsync(logItem.LogLevel);
                    logModel.FullMessage = logItem.FullMessage ?? string.Empty;
                    logModel.ShortMessage = logItem.ShortMessage ?? string.Empty;
                    logModel.CustomerEmail = (await _customerService.GetCustomerByIdAsync(logItem.CustomerId ?? 0))?.Email ?? string.Empty;

                    return logModel;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.CustomerEmail.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.ShortMessage.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.FullMessage.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            //var customer = await _workContext.GetCurrentCustomerAsync();
            //var filterModel = await _persistStateService.GetModelInstance<LogFilterModel>();

            //if (filterModel.LogLevelId > 0)
            //    query = query.Where(x => x.LogLevelId == filterModel.LogLevelId);

            //get parameters to filter log
            //var createdOnFromValue = filterModel.CreatedOnFrom.HasValue
            //    ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(filterModel.CreatedOnFrom.Value) : null;
            //var createdToFromValue = filterModel.CreatedOnTo.HasValue
            //    ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(filterModel.CreatedOnTo.Value).AddDays(1) : null;

            //if (createdOnFromValue.HasValue)
            //    query = query.Where(l => createdOnFromValue.Value <= l.CreatedOnUtc);

            //if (createdToFromValue.HasValue)
            //    query = query.Where(l => createdToFromValue.Value >= l.CreatedOnUtc);

            ////check customer is Administrator
            //if (customer.SystemName != NopCustomerDefaults.AdministratorsRoleName)
            //    query = query.Where(x => x.CustomerId == customer.Id);

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<LogListModel> PrepareLogListModelAsync(LogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var logs = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new LogListModel().PrepareToGrid(searchModel, logs);

            return model;
        }

        public virtual async Task<LogModel> PrepareLogModelAsync(LogModel model, Log log, bool excludeProperties = false)
        {
            if (log != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = log.ToModel<LogModel>();

                    model.LogLevel = await _localizationService.GetLocalizedEnumAsync(log.LogLevel);
                    model.ShortMessage = _htmlFormatter.FormatText(log.ShortMessage, false, true, false, false, false, false);
                    model.FullMessage = _htmlFormatter.FormatText(log.FullMessage, false, true, false, false, false, false);
                    model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc);
                    model.CustomerEmail = log.CustomerId.HasValue ? (await _customerService.GetCustomerByIdAsync(log.CustomerId.Value))?.Email : string.Empty;
                }
            }
            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<LogModel>(1, nameof(LogModel.LogLevelName), ColumnType.RouterLink),
                ColumnConfig.Create<LogModel>(2, nameof(LogModel.ShortMessage)),
                ColumnConfig.Create<LogModel>(3, nameof(LogModel.IpAddress)),
                ColumnConfig.Create<LogModel>(4, nameof(LogModel.CustomerEmail)),
                ColumnConfig.Create<LogModel>(5, nameof(LogModel.PageUrl)),
                ColumnConfig.Create<LogModel>(6, nameof(LogModel.ReferrerUrl)),
                ColumnConfig.Create<LogModel>(7, nameof(LogModel.CreatedOn), ColumnType.Date)
            };

            return columns;
        }

        public virtual async Task<LogFormModel> PrepareLogFormModelAsync(LogFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<LogModel>(nameof(LogModel.LogLevelName), FieldType.Text, _readonly: true),
                FieldConfig.Create<LogModel>(nameof(LogModel.IpAddress), FieldType.Text, _readonly: true),
                FieldConfig.Create<LogModel>(nameof(LogModel.CustomerEmail), FieldType.Text, _readonly: true),
                FieldConfig.Create<LogModel>(nameof(LogModel.PageUrl), FieldType.Text, _readonly: true),
                FieldConfig.Create<LogModel>(nameof(LogModel.ReferrerUrl), FieldType.Text, _readonly: true),
                FieldConfig.Create<LogModel>(nameof(LogModel.CreatedOn), FieldType.DateTime, _readonly: true),
                FieldConfig.Create<LogModel>(nameof(LogModel.ShortMessage), FieldType.Text, _readonly: true),
                FieldConfig.Create<LogModel>(nameof(LogModel.FullMessage), FieldType.Textarea, _readonly: true)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.LogModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

    }
}