using App.Core;
using App.Core.Domain.Scripts;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Accounting;
using App.Models.Scripts;
using App.Services;
using App.Services.Localization;
using App.Services.Scripts;
using App.Services.Traders;
using App.Web.Infra.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptTableItemModelFactory
    {
        Task<ScriptTableItemSearchModel> PrepareScriptTableItemSearchModelAsync(ScriptTableItemSearchModel searchModel);
        Task<ScriptTableItemListModel> PrepareScriptTableItemListModelAsync(ScriptTableItemSearchModel searchModel, int parentId);
        Task<ScriptTableItemModel> PrepareScriptTableItemModelAsync(ScriptTableItemModel model, ScriptTableItem scriptTableItem);
        Task<ScriptTableItemFormModel> PrepareScriptTableItemFormModelAsync(ScriptTableItemFormModel formModel, int traderId);
    }
    public partial class ScriptTableItemModelFactory : IScriptTableItemModelFactory
    {
        private readonly IScriptTableItemService _scriptTableItemService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptTableItemModelFactory(
            IScriptTableItemService scriptTableItemService,
            ITraderConnectionService traderConnectionService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptTableItemService = scriptTableItemService;
            _traderConnectionService = traderConnectionService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptTableItemSearchModel> PrepareScriptTableItemSearchModelAsync(ScriptTableItemSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptTableItemModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptTableItemListModel> PrepareScriptTableItemListModelAsync(ScriptTableItemSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scriptTableItems = await _scriptTableItemService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new ScriptTableItemListModel().PrepareToGrid(searchModel, scriptTableItems);

            return model;
        }

        public virtual Task<ScriptTableItemModel> PrepareScriptTableItemModelAsync(ScriptTableItemModel model, ScriptTableItem scriptTableItem)
        {
            if (scriptTableItem != null)
            {
                //fill in model values from the entity
                model ??= scriptTableItem.ToModel<ScriptTableItemModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScriptTableItemModel>(1, nameof(ScriptTableItemModel.AccountingCode), ColumnType.Link),
                ColumnConfig.Create<ScriptTableItemModel>(1, nameof(ScriptTableItemModel.ScriptBehaviorTypeName))
            };

            return columns;
        }
        
        private async Task<List<SelectionList>> GetAccountingCodesAsync(int traderId) 
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (!connectionResult.Success)
                return new List<SelectionList>();

            var codes = await _dataProvider.QueryAsync<AccountingCodePerSchemaModel>(connectionResult.Connection, SoftOneQuery.AccountingCodesPerSchema);

            var list = codes.Select(x => new SelectionList { Label = x.AccountingCode, Value = $"{x.AccountingCode} - {x.Description}" }).ToList();

            return list;
        }

        public virtual async Task<ScriptTableItemFormModel> PrepareScriptTableItemFormModelAsync(ScriptTableItemFormModel formModel, int traderId)
        {
            var behaviors = await ScriptBehaviorType.Included.ToSelectionItemListAsync();
            var codes = await GetAccountingCodesAsync(traderId);

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptTableItemModel>(nameof(ScriptTableItemModel.AccountingCode), FieldType.Autocomplete, required: true, options: codes),
                FieldConfig.Create<ScriptTableItemModel>(nameof(ScriptTableItemModel.ScriptBehaviorTypeId), FieldType.Select, options: behaviors)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptTableItemModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}