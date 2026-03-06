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
    public partial interface IChamberModelFactory
    {
        Task<ChamberSearchModel> PrepareChamberSearchModelAsync(ChamberSearchModel searchModel);
        Task<ChamberListModel> PrepareChamberListModelAsync(ChamberSearchModel searchModel);
        Task<ChamberModel> PrepareChamberModelAsync(ChamberModel model, Chamber chamber);
        Task<ChamberFormModel> PrepareChamberFormModelAsync(ChamberFormModel formModel);
    }
    public partial class ChamberModelFactory : IChamberModelFactory
    {
        private readonly IChamberService _chamberService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ChamberModelFactory(IChamberService chamberService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _chamberService = chamberService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<ChamberModel>> GetPagedListAsync(ChamberSearchModel searchModel)
        {
            var query = _chamberService.Table.AsEnumerable()
                .Select(x => x.ToModel<ChamberModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.ChamberName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<ChamberSearchModel> PrepareChamberSearchModelAsync(ChamberSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ChamberModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ChamberListModel> PrepareChamberListModelAsync(ChamberSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var chambers = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new ChamberListModel().PrepareToGrid(searchModel, chambers);

            return model;
        }

        public virtual Task<ChamberModel> PrepareChamberModelAsync(ChamberModel model, Chamber chamber)
        {
            if (chamber != null)
            {
                //fill in model values from the entity
                model ??= chamber.ToModel<ChamberModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ChamberModel>(1, nameof(ChamberModel.ChamberName), ColumnType.RouterLink),
                ColumnConfig.Create<ChamberModel>(2, nameof(ChamberModel.ChamberNumber)),
                ColumnConfig.Create<ChamberModel>(2, nameof(ChamberModel.Address), hidden: true),
                ColumnConfig.Create<ChamberModel>(2, nameof(ChamberModel.Postcode), hidden: true),
                ColumnConfig.Create<ChamberModel>(2, nameof(ChamberModel.City)),
                ColumnConfig.Create<ChamberModel>(2, nameof(ChamberModel.PhoneNumber), hidden: true),
                ColumnConfig.Create<ChamberModel>(2, nameof(ChamberModel.Fax), hidden: true),
                ColumnConfig.Create<ChamberModel>(2, nameof(ChamberModel.Email)),
                ColumnConfig.Create<ChamberModel>(2, nameof(ChamberModel.WebPage)),
                ColumnConfig.Create<ChamberModel>(2, nameof(ChamberModel.DisplayOrder), hidden: true)
            };

            return columns;
        }

        public virtual async Task<ChamberFormModel> PrepareChamberFormModelAsync(ChamberFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ChamberModel>(nameof(ChamberModel.ChamberName), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<ChamberModel>(nameof(ChamberModel.ChamberNumber), FieldType.Text),
                FieldConfig.Create<ChamberModel>(nameof(ChamberModel.Address), FieldType.Text),
                FieldConfig.Create<ChamberModel>(nameof(ChamberModel.Postcode), FieldType.Text),
                FieldConfig.Create<ChamberModel>(nameof(ChamberModel.City), FieldType.Text),
                FieldConfig.Create<ChamberModel>(nameof(ChamberModel.PhoneNumber), FieldType.Text),
                FieldConfig.Create<ChamberModel>(nameof(ChamberModel.Fax), FieldType.Text),
                FieldConfig.Create<ChamberModel>(nameof(ChamberModel.Email), FieldType.Text),
                FieldConfig.Create<ChamberModel>(nameof(ChamberModel.WebPage), FieldType.Text),
                FieldConfig.Create<ChamberModel>(nameof(ChamberModel.DisplayOrder), FieldType.Text)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ChamberModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}