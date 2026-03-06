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
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptPivotItemModelFactory
    {
        Task<ScriptPivotItemSearchModel> PrepareScriptPivotItemSearchModelAsync(ScriptPivotItemSearchModel searchModel);
        Task<ScriptPivotItemListModel> PrepareScriptPivotItemListModelAsync(ScriptPivotItemSearchModel searchModel, int parentId);
        Task<ScriptPivotItemModel> PrepareScriptPivotItemModelAsync(ScriptPivotItemModel model, ScriptPivotItem scriptPivotItem);
        Task<ScriptPivotItemFormModel> PrepareScriptPivotItemFormModelAsync(ScriptPivotItemFormModel formModel, int parentId, int traderId);
    }
    public partial class ScriptPivotItemModelFactory : IScriptPivotItemModelFactory
    {
        private readonly IScriptPivotItemService _scriptPivotItemService;
        private readonly IScriptService _scriptService;
        private readonly IScriptFieldService _scriptFieldService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptPivotItemModelFactory(
            IScriptPivotItemService scriptPivotItemService,
            IScriptService scriptService,
            IScriptFieldService scriptFieldService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptPivotItemService = scriptPivotItemService;
            _scriptService = scriptService;
            _scriptFieldService = scriptFieldService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptPivotItemSearchModel> PrepareScriptPivotItemSearchModelAsync(ScriptPivotItemSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptPivotItemModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptPivotItemListModel> PrepareScriptPivotItemListModelAsync(ScriptPivotItemSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scriptPivotItems = await _scriptPivotItemService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new ScriptPivotItemListModel().PrepareToGrid(searchModel, scriptPivotItems);

            return model;
        }

        public virtual Task<ScriptPivotItemModel> PrepareScriptPivotItemModelAsync(ScriptPivotItemModel model, ScriptPivotItem scriptPivotItem)
        {
            if (scriptPivotItem != null)
            {
                //fill in model values from the entity
                model ??= scriptPivotItem.ToModel<ScriptPivotItemModel>();
            }

            if (scriptPivotItem == null)
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
                ColumnConfig.Create<ScriptPivotItemModel>(2, nameof(ScriptPivotItemModel.ScriptFieldName), ColumnType.Link),
                //ColumnConfig.Create<ScriptPivotItemModel>(2, nameof(ScriptPivotItemModel.ScriptOperationTypeName)),
                ColumnConfig.Create<ScriptPivotItemModel>(2, nameof(ScriptPivotItemModel.ScriptFieldTypeName)),
                ColumnConfig.Create<ScriptPivotItemModel>(2, nameof(ScriptPivotItemModel.ScriptDetailName)),
                ColumnConfig.Create<ScriptPivotItemModel>(2, nameof(ScriptPivotItemModel.ParentGroupName), hidden: true),
                ColumnConfig.Create<ScriptPivotItemModel>(2, nameof(ScriptPivotItemModel.Order)),
                ColumnConfig.Create<ScriptPivotItemModel>(2, nameof(ScriptPivotItemModel.Printed), ColumnType.Checkbox)
            };

            return columns;
        }

        public virtual async Task<ScriptPivotItemFormModel> PrepareScriptPivotItemFormModelAsync(ScriptPivotItemFormModel formModel, int parentId, int traderId)
        {
            //var operations = await ScriptOperationType.Addition.ToSelectionItemListAsync();
            var scripts = await ScriptType.Field.ToSelectionItemListAsync();
            var _fields = (await _scriptFieldService.GetAllScriptFieldsAsync(traderId))
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.FieldName }).ToList();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptPivotItemModel>(nameof(ScriptPivotItemModel.ScriptFieldId), FieldType.Select, required: true, options: _fields),
                //FieldConfig.Create<ScriptPivotItemModel>(nameof(ScriptPivotItemModel.ScriptOperationTypeId), FieldType.Select, options: operations),
                FieldConfig.Create<ScriptPivotItemModel>(nameof(ScriptPivotItemModel.Printed), FieldType.Checkbox),
                FieldConfig.Create<ScriptPivotItemModel>(nameof(ScriptPivotItemModel.Order), FieldType.Numeric),
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptPivotItemModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}