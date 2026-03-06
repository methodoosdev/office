using App.Core;
using App.Core.Domain.Employees;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models;
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
    public partial interface ISpecialtyModelFactory
    {
        Task<SpecialtySearchModel> PrepareSpecialtySearchModelAsync(SpecialtySearchModel searchModel);
        Task<SpecialtyListModel> PrepareSpecialtyListModelAsync(SpecialtySearchModel searchModel);
        Task<SpecialtyModel> PrepareSpecialtyModelAsync(SpecialtyModel model, Specialty specialty);
        Task<SpecialtyFormModel> PrepareSpecialtyFormModelAsync(SpecialtyFormModel formModel);
    }
    public partial class SpecialtyModelFactory : ISpecialtyModelFactory
    {
        private readonly ISpecialtyService _specialtyService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public SpecialtyModelFactory(ISpecialtyService specialtyService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _specialtyService = specialtyService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<SpecialtyModel>> GetPagedListAsync(SpecialtySearchModel searchModel)
        {
            var query = _specialtyService.Table.AsEnumerable()
                .Select(x => x.ToModel<SpecialtyModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<SpecialtySearchModel> PrepareSpecialtySearchModelAsync(SpecialtySearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.SpecialtyModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<SpecialtyListModel> PrepareSpecialtyListModelAsync(SpecialtySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var specialties = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new SpecialtyListModel().PrepareToGrid(searchModel, specialties);

            return model;
        }

        public virtual Task<SpecialtyModel> PrepareSpecialtyModelAsync(SpecialtyModel model, Specialty specialty)
        {
            if (specialty != null)
            {
                //fill in model values from the entity
                model ??= specialty.ToModel<SpecialtyModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<SpecialtyModel>(1, nameof(SpecialtyModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<SpecialtyModel>(2, nameof(SpecialtyModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<SpecialtyFormModel> PrepareSpecialtyFormModelAsync(SpecialtyFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SpecialtyModel>(nameof(SpecialtyModel.Description), FieldType.Text),
                FieldConfig.Create<SpecialtyModel>(nameof(SpecialtyModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.SpecialtyModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}