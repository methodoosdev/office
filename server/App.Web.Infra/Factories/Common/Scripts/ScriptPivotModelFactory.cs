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

namespace App.Web.Infra.Factories.Common.ScriptPivots
{
    public partial interface IScriptPivotModelFactory
    {
        Task<ScriptPivotSearchModel> PrepareScriptPivotSearchModelAsync(ScriptPivotSearchModel searchModel);
        Task<ScriptPivotListModel> PrepareScriptPivotListModelAsync(ScriptPivotSearchModel searchModel, int parentId);
        Task<ScriptPivotModel> PrepareScriptPivotModelAsync(ScriptPivotModel model, ScriptPivot scriptPivot);
        Task<ScriptPivotFormModel> PrepareScriptPivotFormModelAsync(ScriptPivotFormModel formModel, int traderId);
        Task<ScriptPivotConfigModel> PrepareScriptPivotConfigModelAsync(ScriptPivotConfigModel configModel, int traderId);
    }
    public partial class ScriptPivotModelFactory : IScriptPivotModelFactory
    {
        private readonly IScriptGroupService _scriptGroupService;
        private readonly IScriptPivotService _scriptPivotService;
        private readonly IScriptFieldService _scriptFieldService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptPivotModelFactory(
            IScriptGroupService scriptGroupService,
            IScriptPivotService scriptPivotService,
            IScriptFieldService scriptFieldService,
            ITraderConnectionService traderConnectionService,
            ISoftoneQueryFactory softoneQueryFactory,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptGroupService = scriptGroupService;
            _scriptPivotService = scriptPivotService;
            _scriptFieldService = scriptFieldService;
            _traderConnectionService = traderConnectionService;
            _softoneQueryFactory = softoneQueryFactory; 
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptPivotSearchModel> PrepareScriptPivotSearchModelAsync(ScriptPivotSearchModel searchModel)
        {
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize(100);
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Group.Add(new GridGroupDescriptor { Field = "scriptGroupName", Dir = "asc" });

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptPivotModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptPivotListModel> PrepareScriptPivotListModelAsync(ScriptPivotSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scriptPivots = await _scriptPivotService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new ScriptPivotListModel().PrepareToGrid(searchModel, scriptPivots);

            return model;
        }

        public virtual Task<ScriptPivotModel> PrepareScriptPivotModelAsync(ScriptPivotModel model, ScriptPivot scriptPivot)
        {
            if (scriptPivot != null)
            {
                //fill in model values from the entity
                model ??= scriptPivot.ToModel<ScriptPivotModel>();
            }

            return Task.FromResult(model);
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var justifyContent = new Dictionary<string, string> { ["justify-content"] = "center" };
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScriptPivotModel>(1, nameof(ScriptPivotModel.ScriptPivotName), ColumnType.Link),
                ColumnConfig.Create<ScriptPivotModel>(2, nameof(ScriptPivotModel.ScriptGroupName), hidden: true),
                ColumnConfig.Create<ScriptPivotModel>(2, nameof(ScriptPivotModel.Description)),
                ColumnConfig.Create<ScriptPivotModel>(2, nameof(ScriptPivotModel.Order)),
                ColumnConfig.CreateButton<ScriptPivotModel>(0, ColumnType.RowButton, "pivot", "primary",
                    await _localizationService.GetResourceAsync("App.Common.Print"), textAlign, justifyContent)
            };

            return columns;
        }

        public virtual async Task<ScriptPivotFormModel> PrepareScriptPivotFormModelAsync(ScriptPivotFormModel formModel, int traderId)
        {
            var scriptFields = await _scriptFieldService.GetAllScriptFieldsAsync(traderId);
            var scriptGroups = (await _scriptGroupService.GetAllScriptGroupsAsync(traderId))
                .OrderBy(x => x.GroupName)
                .Select(x => new SelectionItemList(x.Id, x.GroupName)).ToList();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptPivotModel>(nameof(ScriptPivotModel.ScriptPivotName), FieldType.Text, required: true),
                //FieldConfig.Create<ScriptPivotModel>(nameof(ScriptPivotModel.Group), FieldType.Text),
                FieldConfig.Create<ScriptPivotModel>(nameof(ScriptPivotModel.ScriptGroupId), FieldType.Select, options: scriptGroups),
                FieldConfig.Create<ScriptPivotModel>(nameof(ScriptPivotModel.Description), FieldType.Text),
                FieldConfig.Create<ScriptPivotModel>(nameof(ScriptPivotModel.Order), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptPivotModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        public virtual async Task<ScriptPivotConfigModel> PrepareScriptPivotConfigModelAsync(ScriptPivotConfigModel configModel, int traderId)
        {
            var years = new List<SelectionItemList>();
            var periods = new List<SelectionItemList>();
            var showTypes = await ScriptPivotShowType.Period.ToSelectionItemListAsync();

            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (connectionResult.Success)
            {
                var results = await _softoneQueryFactory.FiscalPeriodPerYearAsync(connectionResult.Connection, connectionResult.CompanyId);
                years = results.Years;
                periods = results.Periods;

                configModel.Year = results.Year;
                configModel.Period = results.Period;
                configModel.Inventory = true;
            }

            var right1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptPivotConfigModel>(nameof(ScriptPivotConfigModel.Year), FieldType.Select, options: years)
            };

            var right2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptPivotConfigModel>(nameof(ScriptPivotConfigModel.Period), FieldType.Select, options: periods)
            };

            var right3 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptPivotConfigModel>(nameof(ScriptPivotConfigModel.ShowTypeId), FieldType.Select, options: showTypes)
            };

            var right4 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptPivotConfigModel>(nameof(ScriptPivotConfigModel.Inventory), FieldType.Checkbox)
            };

            var fields = FieldConfig.CreateFields(new string[] 
            { "col-12 md:col-3", "col-12 md:col-3", "col-12 md:col-3", "col-12 md:col-3" }, right1, right2, right3,right4);

            configModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return configModel;
        }

    }
}