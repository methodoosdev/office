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
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptModelFactory
    {
        Task<ScriptSearchModel> PrepareScriptSearchModelAsync(ScriptSearchModel searchModel);
        Task<ScriptListModel> PrepareScriptListModelAsync(ScriptSearchModel searchModel, int parentId);
        Task<ScriptModel> PrepareScriptModelAsync(ScriptModel model, Script script);
        Task<ScriptFormModel> PrepareScriptFormModelAsync(ScriptFormModel formModel, int traderId);
    }
    public partial class ScriptModelFactory : IScriptModelFactory
    {
        private readonly IScriptGroupService _scriptGroupService;
        private readonly IScriptService _scriptService;
        private readonly IScriptFieldService _scriptFieldService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptModelFactory(
            IScriptGroupService scriptGroupService,
            IScriptService scriptService,
            IScriptFieldService scriptFieldService,
            ITraderConnectionService traderConnectionService,
            ISoftoneQueryFactory softoneQueryFactory,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptGroupService = scriptGroupService;
            _scriptService = scriptService;
            _scriptFieldService = scriptFieldService;
            _traderConnectionService = traderConnectionService;
            _softoneQueryFactory = softoneQueryFactory; 
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptSearchModel> PrepareScriptSearchModelAsync(ScriptSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize(100);
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Group.Add(new GridGroupDescriptor { Field = "scriptGroupName", Dir = "asc" });

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptListModel> PrepareScriptListModelAsync(ScriptSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scripts = await _scriptService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new ScriptListModel().PrepareToGrid(searchModel, scripts);

            return model;
        }

        public virtual Task<ScriptModel> PrepareScriptModelAsync(ScriptModel model, Script script)
        {
            if (script != null)
            {
                //fill in model values from the entity
                model ??= script.ToModel<ScriptModel>();
            }

            if (script == null)
            {
                //fill in model values
                model.Printed = true;
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScriptModel>(1, nameof(ScriptModel.ScriptName), ColumnType.Link),
                ColumnConfig.Create<ScriptModel>(2, nameof(ScriptModel.Replacement)),
                ColumnConfig.Create<ScriptModel>(2, nameof(ScriptModel.ScriptGroupName), hidden: true),
                ColumnConfig.Create<ScriptModel>(2, nameof(ScriptModel.Description), hidden: true),
                ColumnConfig.Create<ScriptModel>(2, nameof(ScriptModel.Order)),
                ColumnConfig.Create<ScriptModel>(2, nameof(ScriptModel.IsPercent), ColumnType.Checkbox),
                ColumnConfig.Create<ScriptModel>(2, nameof(ScriptModel.Printed), ColumnType.Checkbox)
            };

            return columns;
        }

        public virtual async Task<ScriptFormModel> PrepareScriptFormModelAsync(ScriptFormModel formModel, int traderId)
        {
            var scriptFields = await _scriptFieldService.GetAllScriptFieldsAsync(traderId);
            var scriptGroups = (await _scriptGroupService.GetAllScriptGroupsAsync(traderId))
                .OrderBy(x => x.GroupName)
                .Select(x => new SelectionItemList(x.Id, x.GroupName)).ToList();

            var aligns = await ScriptAlignType.Left.ToSelectionItemListAsync();

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptModel>(nameof(ScriptModel.ScriptName), FieldType.Text, required: true),
                FieldConfig.Create<ScriptModel>(nameof(ScriptModel.ScriptGroupId), FieldType.GridSelect, options: scriptGroups),
                FieldConfig.Create<ScriptModel>(nameof(ScriptModel.Replacement), FieldType.TextButton),
                FieldConfig.Create<ScriptModel>(nameof(ScriptModel.IsPercent), FieldType.Checkbox),
                FieldConfig.Create<ScriptModel>(nameof(ScriptModel.Printed), FieldType.Checkbox)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptModel>(nameof(ScriptModel.Description), FieldType.Textarea, rows: 8),
                FieldConfig.Create<ScriptModel>(nameof(ScriptModel.Order), FieldType.Numeric),
                //FieldConfig.Create<ScriptModel>(nameof(ScriptModel.ScriptCode), FieldType.Text),
                //FieldConfig.Create<ScriptModel>(nameof(ScriptModel.ScriptAlignTypeId), FieldType.Select, options: aligns),
                //FieldConfig.Create<ScriptModel>(nameof(ScriptModel.HasHeader), FieldType.Checkbox),
                //FieldConfig.Create<ScriptModel>(nameof(ScriptModel.HeaderCode), FieldType.Text, hideExpression: "model.hasHeader !== true"),
                //FieldConfig.Create<ScriptModel>(nameof(ScriptModel.Header), FieldType.Text, hideExpression: "model.hasHeader !== true"),
                //FieldConfig.Create<ScriptModel>(nameof(ScriptModel.HeaderLeft), FieldType.Text, hideExpression: "model.hasHeader !== true"),
                //FieldConfig.Create<ScriptModel>(nameof(ScriptModel.HeaderRight), FieldType.Text, hideExpression: "model.hasHeader !== true")
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}