using App.Core;
using App.Core.Domain.SimpleTask;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.SimpleTask;
using App.Services.Localization;
using App.Services.SimpleTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.SimpleTask
{
    public partial interface ISimpleTaskDepartmentModelFactory
    {
        Task<SimpleTaskDepartmentSearchModel> PrepareSimpleTaskDepartmentSearchModelAsync(SimpleTaskDepartmentSearchModel searchModel);
        Task<SimpleTaskDepartmentListModel> PrepareSimpleTaskDepartmentListModelAsync(SimpleTaskDepartmentSearchModel searchModel);
        Task<SimpleTaskDepartmentModel> PrepareSimpleTaskDepartmentModelAsync(SimpleTaskDepartmentModel model, SimpleTaskDepartment simpleTaskDepartment);
        Task<SimpleTaskDepartmentFormModel> PrepareSimpleTaskDepartmentFormModelAsync(SimpleTaskDepartmentFormModel formModel);
    }
    public partial class SimpleTaskDepartmentModelFactory : ISimpleTaskDepartmentModelFactory
    {
        private readonly ISimpleTaskDepartmentService _simpleTaskDepartmentService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public SimpleTaskDepartmentModelFactory(ISimpleTaskDepartmentService simpleTaskDepartmentService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _simpleTaskDepartmentService = simpleTaskDepartmentService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<SimpleTaskDepartmentModel>> GetPagedListAsync(SimpleTaskDepartmentSearchModel searchModel)
        {
            var query = _simpleTaskDepartmentService.Table.AsEnumerable()
                .Select(x => x.ToModel<SimpleTaskDepartmentModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<SimpleTaskDepartmentSearchModel> PrepareSimpleTaskDepartmentSearchModelAsync(SimpleTaskDepartmentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customer = await _workContext.GetCurrentCustomerAsync();

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.SimpleTaskDepartmentModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<SimpleTaskDepartmentListModel> PrepareSimpleTaskDepartmentListModelAsync(SimpleTaskDepartmentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var simpleTaskDepartments = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new SimpleTaskDepartmentListModel().PrepareToGrid(searchModel, simpleTaskDepartments);

            return model;
        }

        public virtual Task<SimpleTaskDepartmentModel> PrepareSimpleTaskDepartmentModelAsync(SimpleTaskDepartmentModel model, SimpleTaskDepartment simpleTaskDepartment)
        {
            if (simpleTaskDepartment != null)
            {
                //fill in model values from the entity
                model ??= simpleTaskDepartment.ToModel<SimpleTaskDepartmentModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<SimpleTaskDepartmentModel>(1, nameof(SimpleTaskDepartmentModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<SimpleTaskDepartmentModel>(2, nameof(SimpleTaskDepartmentModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<SimpleTaskDepartmentFormModel> PrepareSimpleTaskDepartmentFormModelAsync(SimpleTaskDepartmentFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskDepartmentModel>(nameof(SimpleTaskDepartmentModel.Description), FieldType.Text),
                FieldConfig.Create<SimpleTaskDepartmentModel>(nameof(SimpleTaskDepartmentModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.SimpleTaskDepartmentModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}