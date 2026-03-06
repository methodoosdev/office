using App.Core;
using App.Core.Domain.Offices;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Offices;
using App.Services.Localization;
using App.Services.Offices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Offices
{

    public partial interface ITaxFactorModelFactory
    {
        Task<TaxFactorSearchModel> PrepareTaxFactorSearchModelAsync(TaxFactorSearchModel searchModel);
        Task<TaxFactorListModel> PrepareTaxFactorListModelAsync(TaxFactorSearchModel searchModel);
        Task<TaxFactorModel> PrepareTaxFactorModelAsync(TaxFactorModel model, TaxFactor taxFactor);
        Task<TaxFactorFormModel> PrepareTaxFactorFormModelAsync(TaxFactorFormModel formModel);
    }

    public partial class TaxFactorModelFactory : ITaxFactorModelFactory
    {
        private readonly ITaxFactorService _taxFactorService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TaxFactorModelFactory(ITaxFactorService taxFactorService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _taxFactorService = taxFactorService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<TaxFactorModel>> GetPagedListAsync(TaxFactorSearchModel searchModel)
        {
            var query = _taxFactorService.Table.AsEnumerable()
              .Select(x => x.ToModel<TaxFactorModel>())
              .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Year.ToString().Contains(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<TaxFactorSearchModel> PrepareTaxFactorSearchModelAsync(TaxFactorSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customer = await _workContext.GetCurrentCustomerAsync();

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TaxFactorModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;

        }
        public virtual async Task<TaxFactorListModel> PrepareTaxFactorListModelAsync(TaxFactorSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var taxfactors = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new TaxFactorListModel().PrepareToGrid(searchModel, taxfactors);

            return model;
        }
        public virtual Task<TaxFactorModel> PrepareTaxFactorModelAsync(TaxFactorModel model, TaxFactor taxFactor)
        {
            if (taxFactor != null)
            {
                //fill in model values from the entity
                model ??= taxFactor.ToModel<TaxFactorModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TaxFactorModel>(1, nameof(TaxFactorModel.Year), ColumnType.RouterLink),
                ColumnConfig.Create<TaxFactorModel>(2, nameof(TaxFactorModel.TaxIncome)),
                ColumnConfig.Create<TaxFactorModel>(3, nameof(TaxFactorModel.TaxAdvance))
            };

            return columns;
        }
        public virtual async Task<TaxFactorFormModel> PrepareTaxFactorFormModelAsync(TaxFactorFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TaxFactorModel>(nameof(TaxFactorModel.Year), FieldType.Text, "number"),
                FieldConfig.Create<TaxFactorModel>(nameof(TaxFactorModel.TaxIncome), FieldType.Numeric),
                FieldConfig.Create<TaxFactorModel>(nameof(TaxFactorModel.TaxAdvance), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TaxFactorModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}
