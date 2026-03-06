using App.Core;
using App.Core.Domain.Scripts;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Scripts;
using App.Services;
using App.Services.Localization;
using App.Services.Scripts;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptToolModelFactory
    {
        Task<ScriptToolSearchModel> PrepareScriptToolSearchModelAsync(ScriptToolSearchModel searchModel);
        Task<ScriptToolListModel> PrepareScriptToolListModelAsync(ScriptToolSearchModel searchModel, int parentId);
        Task<ScriptToolModel> PrepareScriptToolModelAsync(ScriptToolModel model, ScriptTool scriptTool);
        Task<ScriptToolFormModel> PrepareScriptToolFormModelAsync(ScriptToolFormModel formModel, int traderId);
        Task<Dictionary<string, object>> PrepareScriptReplacementAsync(int traderId, string[] tokens, ScriptToolConfigModel config);
        Task<ScriptToolConfigModel> PrepareScriptToolConfigModelAsync(ScriptToolConfigModel configModel, int traderId);
    }
    public partial class ScriptToolModelFactory : IScriptToolModelFactory
    {
        private readonly IScriptService _scriptService;
        private readonly IScriptTraderModelService _scriptTraderModelService;
        private readonly IScriptToolService _scriptToolService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptToolModelFactory(
            IScriptService scriptService,
            IScriptTraderModelService scriptTraderModelService,
            IScriptToolService scriptToolService,
            ITraderConnectionService traderConnectionService,
            ISoftoneQueryFactory softoneQueryFactory,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptService = scriptService;
            _scriptTraderModelService = scriptTraderModelService;
            _scriptToolService = scriptToolService;
            _traderConnectionService = traderConnectionService;
            _softoneQueryFactory = softoneQueryFactory; 
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptToolSearchModel> PrepareScriptToolSearchModelAsync(ScriptToolSearchModel searchModel)
        {
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize(100);
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptToolModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptToolListModel> PrepareScriptToolListModelAsync(ScriptToolSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scriptTools = await _scriptToolService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new ScriptToolListModel().PrepareToGrid(searchModel, scriptTools);

            return model;
        }

        public virtual Task<ScriptToolModel> PrepareScriptToolModelAsync(ScriptToolModel model, ScriptTool scriptTool)
        {
            if (scriptTool != null)
            {
                //fill in model values from the entity
                model ??= scriptTool.ToModel<ScriptToolModel>();
            }

            return Task.FromResult(model);
        }

        private Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var justifyContent = new Dictionary<string, string> { ["justify-content"] = "center" };
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScriptToolModel>(1, nameof(ScriptToolModel.Title), ColumnType.Link),
                ColumnConfig.Create<ScriptToolModel>(2, nameof(ScriptToolModel.Subtitle)),
                ColumnConfig.Create<ScriptToolModel>(2, nameof(ScriptToolModel.Description), hidden: true),
                ColumnConfig.Create<ScriptToolModel>(2, nameof(ScriptToolModel.Order)),
                ColumnConfig.CreateButton<ScriptToolModel>(0, ColumnType.RowButton, "loadPrototype", "primary",
                    "Πρότυπο Excel", textAlign, justifyContent),
                ColumnConfig.CreateButton<ScriptToolModel>(0, ColumnType.RowButton, "downloadPrototype", "warning",
                    "Εξ.Προτύπου", textAlign, justifyContent, hidden: true),
                ColumnConfig.CreateButton<ScriptToolModel>(0, ColumnType.RowButton, "downloadExcel", "info",
                    "Εξαγωγή", textAlign, justifyContent),
                //ColumnConfig.CreateButton<ScriptToolModel>(0, ColumnType.RowButton, "tool", "primary",
                //    await _localizationService.GetResourceAsync("App.Common.Print"), textAlign, justifyContent),
                //ColumnConfig.CreateButton<ScriptToolModel>(0, ColumnType.RowButton, "diagram", "warning",
                //    await _localizationService.GetResourceAsync("App.Common.Diagram"), textAlign, justifyContent)
            };

            return Task.FromResult(columns);
        }

        public virtual async Task<ScriptToolFormModel> PrepareScriptToolFormModelAsync(ScriptToolFormModel formModel, int traderId)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptToolModel>(nameof(ScriptToolModel.Title), FieldType.Text, required: true),
                FieldConfig.Create<ScriptToolModel>(nameof(ScriptToolModel.Subtitle), FieldType.Text),
                FieldConfig.Create<ScriptToolModel>(nameof(ScriptToolModel.Description), FieldType.Textarea, rows: 2),
                FieldConfig.Create<ScriptToolModel>(nameof(ScriptToolModel.Order), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptToolModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        public virtual async Task<Dictionary<string, object>> PrepareScriptReplacementAsync(int traderId, string[] tokens, ScriptToolConfigModel config)
        {
            var dict = await _scriptService.GetScriptIdsByTokensAsync(traderId, tokens);

            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);

            var scriptList = new Dictionary<string, object>();

            if (connectionResult.Success)
            {
                foreach (var item in dict)
                {
                    var previous = item.Key.EndsWith("-");
                    var script = item.Value;

                    var scriptItemsDict = await _scriptTraderModelService.CreateScriptItemsDictAsync(
                        script, traderId, connectionResult.Connection, connectionResult.CompanyId, config, previous);

                    var value = _scriptTraderModelService.CreateScriptsDictItem(scriptItemsDict);

                    scriptList.Add(item.Key, value);
                }
            }

            return scriptList;
        }

        public virtual async Task<ScriptToolConfigModel> PrepareScriptToolConfigModelAsync(ScriptToolConfigModel configModel, int traderId)
        {
            var fiscalYears = await ScriptFiscalYearType.Current.ToSelectionItemListAsync();
            var periods = new List<SelectionItemList>();

            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (connectionResult.Success)
            {
                var results = await _softoneQueryFactory.FiscalPeriodPerYearAsync(connectionResult.Connection, connectionResult.CompanyId);
                periods = results.Periods;
            }

            //var month = DateTime.UtcNow.Month;
            //configModel.PeriodFrom = 1;
            //configModel.PeriodTo = month;
            configModel.Active = true;
            configModel.PeriodFrom = 1;
            configModel.PeriodTo = 12;
            configModel.Inventory = true;

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptToolConfigModel>(nameof(ScriptToolConfigModel.Active), FieldType.Checkbox, className: "col-12 md:col-6")
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptToolConfigModel>(nameof(ScriptToolConfigModel.FiscalYear), FieldType.Select, options: fiscalYears, className: "col-12 md:col-3", disableExpression: "model.active !== true"),
                FieldConfig.Create<ScriptToolConfigModel>(nameof(ScriptToolConfigModel.PeriodFrom), FieldType.Select, options: periods, className: "col-12 md:col-3", disableExpression: "model.active !== true"),
                FieldConfig.Create<ScriptToolConfigModel>(nameof(ScriptToolConfigModel.PeriodTo), FieldType.Select, options: periods, className: "col-12 md:col-3", disableExpression: "model.active !== true"),
                FieldConfig.Create<ScriptToolConfigModel>(nameof(ScriptToolConfigModel.Inventory), FieldType.Checkbox, className: "col-12 md:col-3", disableExpression: "model.active !== true")
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12" }, left, right);

            configModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return configModel;
        }

    }
}