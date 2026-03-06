using App.Core;
using App.Core.Domain.Logging;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Logging;
using App.Services;
using App.Services.Localization;
using App.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Admin.Factories.Logging
{
    public partial interface IActivityLogTypeModelFactory
    {
        Task<ActivityLogTypeSearchModel> PrepareActivityLogTypeSearchModelAsync(ActivityLogTypeSearchModel searchModel);
        Task<ActivityLogTypeListModel> PrepareActivityLogTypeListModelAsync(ActivityLogTypeSearchModel searchModel);
        Task<ActivityLogTypeModel> PrepareActivityLogTypeModelAsync(ActivityLogTypeModel model, ActivityLogType activityLogType);
        Task<ActivityLogTypeFormModel> PrepareActivityLogTypeFormModelAsync(ActivityLogTypeFormModel formModel, bool editMode);
    }
    public partial class ActivityLogTypeModelFactory : IActivityLogTypeModelFactory
    {
        private readonly IActivityLogTypeService _activityLogTypeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ActivityLogTypeModelFactory(IActivityLogTypeService activityLogTypeService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _activityLogTypeService = activityLogTypeService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<ActivityLogTypeModel>> GetPagedListAsync(ActivityLogTypeSearchModel searchModel)
        {
            var query = _activityLogTypeService.Table.AsEnumerable()
                .Select(x => x.ToModel<ActivityLogTypeModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.SystemKeyword.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<ActivityLogTypeSearchModel> PrepareActivityLogTypeSearchModelAsync(ActivityLogTypeSearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ActivityLogTypeModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ActivityLogTypeListModel> PrepareActivityLogTypeListModelAsync(ActivityLogTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var activityLogTypes = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new ActivityLogTypeListModel().PrepareToGrid(searchModel, activityLogTypes);

            return model;
        }

        public virtual Task<ActivityLogTypeModel> PrepareActivityLogTypeModelAsync(ActivityLogTypeModel model, ActivityLogType activityLogType)
        {
            if (activityLogType != null)
            {
                //fill in model values from the entity
                model ??= activityLogType.ToModel<ActivityLogTypeModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ActivityLogTypeModel>(1, nameof(ActivityLogTypeModel.SystemKeyword), ColumnType.RouterLink),
                ColumnConfig.Create<ActivityLogTypeModel>(2, nameof(ActivityLogTypeModel.Name)),
                ColumnConfig.Create<ActivityLogTypeModel>(3, nameof(ActivityLogTypeModel.Enabled), ColumnType.Checkbox)
            };

            return columns;
        }

        public virtual async Task<ActivityLogTypeFormModel> PrepareActivityLogTypeFormModelAsync(ActivityLogTypeFormModel formModel, bool editMode)
        {
            var types = await ActivityLogTypeType.ListingF5.ToSelectionListAsync();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ActivityLogTypeModel>(nameof(ActivityLogTypeModel.SystemKeyword), FieldType.Select, options: types, _readonly: editMode),
                FieldConfig.Create<ActivityLogTypeModel>(nameof(ActivityLogTypeModel.Name), FieldType.Text),
                FieldConfig.Create<ActivityLogTypeModel>(nameof(ActivityLogTypeModel.Enabled), FieldType.Switch)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ActivityLogTypeModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}