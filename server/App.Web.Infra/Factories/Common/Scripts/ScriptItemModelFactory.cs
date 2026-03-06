using App.Core;
using App.Core.Domain.Scripts;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Scripts;
using App.Services;
using App.Services.Localization;
using App.Services.Scripts;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptItemModelFactory
    {
        Task<ScriptItemSearchModel> PrepareScriptItemSearchModelAsync(ScriptItemSearchModel searchModel);
        Task<ScriptItemListModel> PrepareScriptItemListModelAsync(ScriptItemSearchModel searchModel, int parentId, int traderId);
        Task<ScriptItemModel> PrepareScriptItemModelAsync(ScriptItemModel model, ScriptItem scriptItem);
        Task<ScriptItemFormModel> PrepareScriptItemFormModelAsync(ScriptItemFormModel formModel, int parentId, int traderId);
    }
    public partial class ScriptItemModelFactory : IScriptItemModelFactory
    {
        private readonly IScriptItemService _scriptItemService;
        private readonly IScriptService _scriptService;
        private readonly IScriptFieldService _scriptFieldService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptItemModelFactory(
            IScriptItemService scriptItemService,
            IScriptService scriptService,
            IScriptFieldService scriptFieldService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptItemService = scriptItemService;
            _scriptService = scriptService;
            _scriptFieldService = scriptFieldService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptItemSearchModel> PrepareScriptItemSearchModelAsync(ScriptItemSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptItemModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptItemListModel> PrepareScriptItemListModelAsync(ScriptItemSearchModel searchModel, int parentId, int traderId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scriptItems = await _scriptItemService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new ScriptItemListModel().PrepareToGrid(searchModel, scriptItems);

            return model;
        }

        public virtual Task<ScriptItemModel> PrepareScriptItemModelAsync(ScriptItemModel model, ScriptItem scriptItem)
        {
            if (scriptItem != null)
            {
                //fill in model values from the entity
                model ??= scriptItem.ToModel<ScriptItemModel>();
            }

            return Task.FromResult(model);
        }
        
        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScriptItemModel>(2, nameof(ScriptItemModel.ScriptTypeName), ColumnType.Link),
                ColumnConfig.Create<ScriptItemModel>(2, nameof(ScriptItemModel.ParentName)),
                ColumnConfig.Create<ScriptItemModel>(2, nameof(ScriptItemModel.ScriptOperationTypeName)),
                ColumnConfig.Create<ScriptItemModel>(2, nameof(ScriptItemModel.ParentGroupName), hidden: true),
                ColumnConfig.Create<ScriptItemModel>(2, nameof(ScriptItemModel.Order))
            };

            return columns;
        }

        public virtual async Task<ScriptItemFormModel> PrepareScriptItemFormModelAsync(ScriptItemFormModel formModel, int parentId, int traderId)
        {
            var operations = await ScriptOperationType.Addition.ToSelectionItemListAsync();
            var scriptTypes = await ScriptType.Field.ToSelectionItemListAsync();
            var script = await _scriptService.GetScriptByIdAsync(parentId);
            var _fields = (await _scriptFieldService.GetAllScriptFieldsAsync(traderId))
                .Where(k => k.ScriptGroupId == script.ScriptGroupId)
                .OrderBy(x => x.FieldName)
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.FieldName }).ToList();
            var _scripts = (await _scriptService.GetAllScriptsAsync(traderId))
                .Where(k => !parentId.Equals(k.Id) && k.ScriptGroupId == script.ScriptGroupId)
                .OrderBy(x => x.ScriptName)
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.ScriptName }).ToList();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptItemModel>(nameof(ScriptItemModel.ScriptTypeId), FieldType.Select, options: scriptTypes),
                FieldConfig.Create<ScriptItemModel>(nameof(ScriptItemModel.ScriptFieldId), FieldType.GridSelect, required: true, options: _fields, hideExpression: "model.scriptTypeId != 0"),
                FieldConfig.Create<ScriptItemModel>(nameof(ScriptItemModel.ParentId), FieldType.GridSelect, options: _scripts, hideExpression: "model.scriptTypeId != 1"),
                FieldConfig.Create<ScriptItemModel>(nameof(ScriptItemModel.ScriptOperationTypeId), FieldType.Select, options: operations),
                FieldConfig.Create<ScriptItemModel>(nameof(ScriptItemModel.Order), FieldType.Numeric),
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptItemModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}