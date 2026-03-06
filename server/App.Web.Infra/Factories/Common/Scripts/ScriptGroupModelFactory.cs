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
    public partial interface IScriptGroupModelFactory
    {
        Task<ScriptGroupSearchModel> PrepareScriptGroupSearchModelAsync(ScriptGroupSearchModel searchModel);
        Task<ScriptGroupListModel> PrepareScriptGroupListModelAsync(ScriptGroupSearchModel searchModel, int parentId);
        Task<ScriptGroupModel> PrepareScriptGroupModelAsync(ScriptGroupModel model, ScriptGroup scriptGroup);
        Task<ScriptGroupFormModel> PrepareScriptGroupFormModelAsync(ScriptGroupFormModel formModel);
    }
    public partial class ScriptGroupModelFactory : IScriptGroupModelFactory
    {
        private readonly IScriptGroupService _scriptGroupService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptGroupModelFactory(
            IScriptGroupService scriptGroupService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptGroupService = scriptGroupService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptGroupSearchModel> PrepareScriptGroupSearchModelAsync(ScriptGroupSearchModel searchModel)
        {
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize(10);
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptGroupModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptGroupListModel> PrepareScriptGroupListModelAsync(ScriptGroupSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scriptGroups = await _scriptGroupService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new ScriptGroupListModel().PrepareToGrid(searchModel, scriptGroups);

            return model;
        }

        public virtual Task<ScriptGroupModel> PrepareScriptGroupModelAsync(ScriptGroupModel model, ScriptGroup scriptGroup)
        {
            if (scriptGroup != null)
            {
                //fill in model values from the entity
                model ??= scriptGroup.ToModel<ScriptGroupModel>();
            }

            return Task.FromResult(model);
        }

        private Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScriptGroupModel>(1, nameof(ScriptGroupModel.GroupName), ColumnType.Link),
                //ColumnConfig.Create<ScriptGroupModel>(2, nameof(ScriptGroupModel.ScriptAlignTypeName)),
                ColumnConfig.Create<ScriptGroupModel>(3, nameof(ScriptGroupModel.Order))
            };

            return Task.FromResult(columns);
        }

        public virtual async Task<ScriptGroupFormModel> PrepareScriptGroupFormModelAsync(ScriptGroupFormModel formModel)
        {
            var aligns = await ScriptAlignType.Left.ToSelectionItemListAsync();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptGroupModel>(nameof(ScriptGroupModel.GroupName), FieldType.Text, required: true),
                //FieldConfig.Create<ScriptGroupModel>(nameof(ScriptGroupModel.ScriptAlignTypeId), FieldType.Select, options: aligns),
                FieldConfig.Create<ScriptGroupModel>(nameof(ScriptGroupModel.Order), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptGroupModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}