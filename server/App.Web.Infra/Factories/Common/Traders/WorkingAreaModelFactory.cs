using App.Core;
using App.Core.Domain.Traders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface IWorkingAreaModelFactory
    {
        Task<WorkingAreaSearchModel> PrepareWorkingAreaSearchModelAsync(WorkingAreaSearchModel searchModel);
        Task<WorkingAreaListModel> PrepareWorkingAreaListModelAsync(WorkingAreaSearchModel searchModel);
        Task<WorkingAreaModel> PrepareWorkingAreaModelAsync(WorkingAreaModel model, WorkingArea workingArea);
        Task<WorkingAreaFormModel> PrepareWorkingAreaFormModelAsync(WorkingAreaFormModel formModel);
    }
    public partial class WorkingAreaModelFactory : IWorkingAreaModelFactory
    {
        private readonly IWorkingAreaService _workingAreaService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public WorkingAreaModelFactory(IWorkingAreaService workingAreaService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _workingAreaService = workingAreaService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<WorkingAreaSearchModel> PrepareWorkingAreaSearchModelAsync(WorkingAreaSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.WorkingAreaModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<WorkingAreaListModel> PrepareWorkingAreaListModelAsync(WorkingAreaSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var workingAreas = await _workingAreaService.GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new WorkingAreaListModel().PrepareToGrid(searchModel, workingAreas);

            return model;
        }

        public virtual Task<WorkingAreaModel> PrepareWorkingAreaModelAsync(WorkingAreaModel model, WorkingArea workingArea)
        {
            if (workingArea != null)
            {
                //fill in model values from the entity
                model ??= workingArea.ToModel<WorkingAreaModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkingAreaModel>(1, nameof(WorkingAreaModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<WorkingAreaModel>(2, nameof(WorkingAreaModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<WorkingAreaFormModel> PrepareWorkingAreaFormModelAsync(WorkingAreaFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkingAreaModel>(nameof(WorkingAreaModel.Description), FieldType.Text),
                FieldConfig.Create<WorkingAreaModel>(nameof(WorkingAreaModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.WorkingAreaModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}