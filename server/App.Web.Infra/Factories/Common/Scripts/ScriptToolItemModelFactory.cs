using App.Core;
using App.Core.Domain.Scripts;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Scripts;
using App.Services;
using App.Services.Localization;
using App.Services.Scripts;
using ExCSS;
using NPOI.XSSF.UserModel;
using Scryber.OpenType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptToolItemModelFactory
    {
        Task<ScriptToolItemSearchModel> PrepareScriptToolItemSearchModelAsync(ScriptToolItemSearchModel searchModel);
        Task<ScriptToolItemListModel> PrepareScriptToolItemListModelAsync(ScriptToolItemSearchModel searchModel, int parentId, int traderId);
        Task<ScriptToolItemModel> PrepareScriptToolItemModelAsync(ScriptToolItemModel model, ScriptToolItem scriptToolItem);
        Task<ScriptToolItemFormModel> PrepareScriptToolItemFormModelAsync(ScriptToolItemFormModel formModel, int parentId, int traderId);
    }
    public partial class ScriptToolItemModelFactory : IScriptToolItemModelFactory
    {
        private readonly IScriptToolItemService _scriptToolItemService;
        private readonly IScriptService _scriptService;
        private readonly IScriptGroupService _scriptGroupService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptToolItemModelFactory(
            IScriptToolItemService scriptToolItemService,
            IScriptService scriptService,
            IScriptGroupService scriptGroupService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptToolItemService = scriptToolItemService;
            _scriptService = scriptService;
            _scriptGroupService = scriptGroupService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptToolItemSearchModel> PrepareScriptToolItemSearchModelAsync(ScriptToolItemSearchModel searchModel)
        {
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Group.Add(new GridGroupDescriptor { Field = "scriptGroupName", Dir = "asc" });

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptToolItemModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptToolItemListModel> PrepareScriptToolItemListModelAsync(ScriptToolItemSearchModel searchModel, int parentId, int traderId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scriptToolItems = await _scriptToolItemService.GetPagedListAsync(searchModel, parentId, traderId);

            //prepare grid model
            var model = new ScriptToolItemListModel().PrepareToGrid(searchModel, scriptToolItems);

            return model;
        }

        public virtual Task<ScriptToolItemModel> PrepareScriptToolItemModelAsync(ScriptToolItemModel model, ScriptToolItem scriptToolItem)
        {
            if (scriptToolItem != null)
            {
                //fill in model values from the entity
                model ??= scriptToolItem.ToModel<ScriptToolItemModel>();
            }

            return Task.FromResult(model);
        }
        
        private Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScriptToolItemModel>(2, nameof(ScriptToolItemModel.ScriptName), ColumnType.Link),
                ColumnConfig.Create<ScriptToolItemModel>(2, nameof(ScriptToolItemModel.ScriptGroupName), hidden: true),
                ColumnConfig.Create<ScriptToolItemModel>(2, nameof(ScriptToolItemModel.Order))
            };

            return Task.FromResult(columns);
        }

        public virtual async Task<ScriptToolItemFormModel> PrepareScriptToolItemFormModelAsync(ScriptToolItemFormModel formModel, int parentId, int traderId)
        {
            var scriptGroupList = await _scriptGroupService.Table.Where(x => x.TraderId == traderId).ToListAsync();
            var scriptList = await _scriptService.Table.Where(x => x.TraderId == traderId).ToListAsync();
            var _scripts = scriptList.Select(x => 
            {
                var scriptGroup = scriptGroupList.FirstOrDefault(k => k.Id == x.ScriptGroupId);

                var model = new ScriptToolItemModel();
                model.ScriptId = x.Id;
                model.ScriptName = x.ScriptName;
                model.ScriptGroupName = scriptGroup?.GroupName ?? "";

                return model;
            }).ToList();

            var scripts = _scripts.Select(x => new SelectionGroupItemList(x.ScriptId, x.ScriptName, x.ScriptGroupName)).ToList();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptToolItemModel>(nameof(ScriptToolItemModel.ScriptId), FieldType.GridGroupSelect, sourceOptions: scripts, groupable: true)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptToolItemModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}