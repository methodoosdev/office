using App.Core;
using App.Core.Domain.Employees;
using App.Core.Domain.VatExemption;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Employees;
using App.Services.Employees;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Employees
{
    public partial interface IDepartmentModelFactory
    {
        Task<DepartmentSearchModel> PrepareDepartmentSearchModelAsync(DepartmentSearchModel searchModel);
        Task<DepartmentListModel> PrepareDepartmentListModelAsync(DepartmentSearchModel searchModel);
        Task<DepartmentModel> PrepareDepartmentModelAsync(DepartmentModel model, Department department);
        Task<DepartmentFormModel> PrepareDepartmentFormModelAsync(DepartmentFormModel formModel);
    }
    public partial class DepartmentModelFactory : IDepartmentModelFactory
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public DepartmentModelFactory(IDepartmentService departmentService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _departmentService = departmentService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<DepartmentModel>> GetPagedListAsync(DepartmentSearchModel searchModel)
        {
            var query = _departmentService.Table.AsEnumerable()
                .Select(x => x.ToModel<DepartmentModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<DepartmentSearchModel> PrepareDepartmentSearchModelAsync(DepartmentSearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.DepartmentModel.ListForm.Title");
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<DepartmentListModel> PrepareDepartmentListModelAsync(DepartmentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var departments = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new DepartmentListModel().PrepareToGrid(searchModel, departments);

            return model;
        }

        public virtual Task<DepartmentModel> PrepareDepartmentModelAsync(DepartmentModel model, Department department)
        {
            if (department != null)
            {
                //fill in model values from the entity
                model ??= department.ToModel<DepartmentModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<DepartmentModel>(1, nameof(DepartmentModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<DepartmentModel>(2, nameof(DepartmentModel.SystemName)),
                ColumnConfig.Create<DepartmentModel>(2, nameof(DepartmentModel.DisplayOrder)),
                ColumnConfig.Create<DepartmentModel>(3, nameof(DepartmentModel.Background)),
                ColumnConfig.Create<DepartmentModel>(4, nameof(DepartmentModel.Color))
            };

            return columns;
        }

        public virtual async Task<DepartmentFormModel> PrepareDepartmentFormModelAsync(DepartmentFormModel formModel)
        {
            var defaultItemText = await _localizationService.GetResourceAsync("App.Common.Choice");

            var departments = new[] {
                DepartmentDefaults.AdministrationName,
                DepartmentDefaults.AccountingName,
                DepartmentDefaults.PayrollName,
                DepartmentDefaults.SecretariatName,
                DepartmentDefaults.DevelopersName,
                DepartmentDefaults.LegalName,
                DepartmentDefaults.Developers2Name,
                DepartmentDefaults.ManagmentName
            }.ToList();

            var systemNames = departments.Select(name => new SelectionList { Label = name, Value = name }).ToList();
            systemNames.Insert(0, new SelectionList { Label = defaultItemText, Value = null });

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<DepartmentModel>(nameof(DepartmentModel.SystemName), FieldType.GridSelect, options: systemNames),
                FieldConfig.Create<DepartmentModel>(nameof(DepartmentModel.Description), FieldType.Text),
                FieldConfig.Create<DepartmentModel>(nameof(DepartmentModel.DisplayOrder), FieldType.Numeric),
                FieldConfig.Create<DepartmentModel>(nameof(DepartmentModel.Background), FieldType.Text),
                FieldConfig.Create<DepartmentModel>(nameof(DepartmentModel.Color), FieldType.Text)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.DepartmentModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}