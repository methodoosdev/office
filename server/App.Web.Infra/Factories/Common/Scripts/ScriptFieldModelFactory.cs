using App.Core;
using App.Core.Domain.Scripts;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Scripts;
using App.Services;
using App.Services.Localization;
using App.Services.Scripts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptFieldModelFactory
    {
        Task<ScriptFieldSearchModel> PrepareScriptFieldSearchModelAsync(ScriptFieldSearchModel searchModel);
        Task<ScriptFieldListModel> PrepareScriptFieldListModelAsync(ScriptFieldSearchModel searchModel, int parentId);
        Task<ScriptFieldModel> PrepareScriptFieldModelAsync(ScriptFieldModel model, ScriptField scriptField);
        Task<ScriptFieldFormModel> PrepareScriptFieldFormModelAsync(ScriptFieldFormModel formModel, int traderId);
    }
    public partial class ScriptFieldModelFactory : IScriptFieldModelFactory
    {
        private readonly IScriptGroupService _scriptGroupService;
        private readonly IScriptFieldService _scriptFieldService;
        private readonly IScriptTableService _scriptTableService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptFieldModelFactory(
            IScriptGroupService scriptGroupService,
            IScriptFieldService scriptFieldService,
            IScriptTableService scriptTableService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptGroupService = scriptGroupService;
            _scriptFieldService = scriptFieldService;
            _scriptTableService = scriptTableService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptFieldSearchModel> PrepareScriptFieldSearchModelAsync(ScriptFieldSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize(100);
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Group.Add(new GridGroupDescriptor { Field = "scriptGroupName", Dir = "asc" });

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptFieldModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptFieldListModel> PrepareScriptFieldListModelAsync(ScriptFieldSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scriptFields = await _scriptFieldService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new ScriptFieldListModel().PrepareToGrid(searchModel, scriptFields);

            return model;
        }

        public virtual Task<ScriptFieldModel> PrepareScriptFieldModelAsync(ScriptFieldModel model, ScriptField scriptField)
        {
            if (scriptField != null)
            {
                //fill in model values from the entity
                model ??= scriptField.ToModel<ScriptFieldModel>();
            }

            if (scriptField == null)
            {
                //fill in model values
                model.PeriodFrom = 1;
                model.PeriodTo = 12;
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScriptFieldModel>(1, nameof(ScriptFieldModel.FieldName), ColumnType.Link),
                ColumnConfig.Create<ScriptFieldModel>(2, nameof(ScriptFieldModel.Description), hidden: true),
                ColumnConfig.Create<ScriptFieldModel>(2, nameof(ScriptFieldModel.ScriptAggregateTypeName)),
                ColumnConfig.Create<ScriptFieldModel>(2, nameof(ScriptFieldModel.ScriptFieldTypeName)),
                ColumnConfig.Create<ScriptFieldModel>(2, nameof(ScriptFieldModel.ScriptDetailName)),
                ColumnConfig.Create<ScriptFieldModel>(2, nameof(ScriptFieldModel.ScriptGroupName), hidden: true),
                ColumnConfig.Create<ScriptFieldModel>(2, nameof(ScriptFieldModel.Locked), ColumnType.Checkbox),
                ColumnConfig.Create<ScriptFieldModel>(2, nameof(ScriptFieldModel.Order))
            };

            return columns;
        }
        public virtual async Task<ScriptFieldFormModel> PrepareScriptFieldFormModelAsync(ScriptFieldFormModel formModel, int traderId)
        {
            var queries = await ScriptQueryType.Payments.ToSelectionItemListAsync();
            var scriptAggregates = await ScriptAggregateType.Sum.ToSelectionItemListAsync();
            var scriptFields = await ScriptFieldType.Table.ToSelectionItemListAsync();
            var fiscalYears = await ScriptFiscalYearType.Current.ToSelectionItemListAsync();
            var functions = await ScriptFunctionType.EmployeesCount.ToSelectionItemListAsync();
            var periods = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                .Select((x, i) => new SelectionItemList { Value = i+1, Label = x }).ToList();
            //periods.Insert(0, new SelectionItemList { Value = 0, Label = await _localizationService.GetResourceAsync("App.Common.Inventory") });

            var scriptTables = (await _scriptTableService.GetAllScriptTablesAsync(traderId))
                .OrderBy(x => x.TableName)
                .Select((x) => new SelectionItemList { Value = x.Id, Label = x.TableName }).ToList();

            var scriptGroups = (await _scriptGroupService.GetAllScriptGroupsAsync(traderId))
                .OrderBy(x => x.GroupName)
                .Select(x => new SelectionItemList(x.Id, x.GroupName)).ToList();

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.FieldName), FieldType.TextButton, required: true),
                //FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.Group), FieldType.Text),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.ScriptGroupId), FieldType.GridSelect, options: scriptGroups),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.Description), FieldType.Text),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.Order), FieldType.Numeric),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.ScriptAggregateTypeId), FieldType.Select, options: scriptAggregates),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.ScriptFieldTypeId), FieldType.Select, options: scriptFields),
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.ScriptQueryTypeId), FieldType.GridSelect, required: true, options: queries, hideExpression: "model.scriptFieldTypeId != 1"),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.ScriptFunctionTypeId), FieldType.GridSelect, required: true, options: functions, hideExpression: "model.scriptFieldTypeId != 2"),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.FixedValue), FieldType.Decimals, hideExpression: "model.scriptFieldTypeId != 3"),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.ScriptTableId), FieldType.GridSelect, required: true, options: scriptTables, hideExpression: "model.scriptFieldTypeId != 0"),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.FiscalYear), FieldType.Select, options: fiscalYears, hideExpression: "model.scriptFieldTypeId > 1"),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.PeriodFrom), FieldType.Select, options: periods, hideExpression: "model.scriptFieldTypeId > 1"),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.PeriodTo), FieldType.Select, options: periods, hideExpression: "model.scriptFieldTypeId > 1"),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.Inventory), FieldType.Checkbox, hideExpression: "model.scriptFieldTypeId > 1"),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.BalanceSheet), FieldType.Checkbox, hideExpression: "model.scriptFieldTypeId > 1"),
                FieldConfig.Create<ScriptFieldModel>(nameof(ScriptFieldModel.Locked), FieldType.Checkbox, hideExpression: "model.scriptFieldTypeId > 1"),
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptFieldModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}