using App.Core;
using App.Core.Domain.Scripts;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Scripts;
using App.Models.Tasks;
using App.Services.Localization;
using App.Services.Scripts;
using DocumentFormat.OpenXml.EMMA;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptTableModelFactory
    {
        Task<ScriptTableSearchModel> PrepareScriptTableSearchModelAsync(ScriptTableSearchModel searchModel);
        Task<ScriptTableListModel> PrepareScriptTableListModelAsync(ScriptTableSearchModel searchModel, int parentId);
        Task<ScriptTableModel> PrepareScriptTableModelAsync(ScriptTableModel model, ScriptTable scriptTable);
        Task<ScriptTableFormModel> PrepareScriptTableFormModelAsync(ScriptTableFormModel formModel, int traderId);
    }
    public partial class ScriptTableModelFactory : IScriptTableModelFactory
    {
        private readonly IScriptGroupService _scriptGroupService;
        private readonly IScriptTableService _scriptTableService;
        private readonly IScriptTableNameService _scriptTableNameService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptTableModelFactory(
            IScriptGroupService scriptGroupService,
            IScriptTableService scriptTableService,
            IScriptTableNameService scriptTableNameService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptGroupService = scriptGroupService;
            _scriptTableService = scriptTableService;
            _scriptTableNameService = scriptTableNameService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptTableSearchModel> PrepareScriptTableSearchModelAsync(ScriptTableSearchModel searchModel)
        {
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize(100);
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Group.Add(new GridGroupDescriptor { Field = "scriptGroupName", Dir = "asc" });

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptTableModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptTableListModel> PrepareScriptTableListModelAsync(ScriptTableSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scriptTables = await _scriptTableService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new ScriptTableListModel().PrepareToGrid(searchModel, scriptTables);

            return model;
        }

        public virtual Task<ScriptTableModel> PrepareScriptTableModelAsync(ScriptTableModel model, ScriptTable scriptTable)
        {
            if (scriptTable != null)
            {
                //fill in model values from the entity
                model ??= scriptTable.ToModel<ScriptTableModel>();
            }

            return Task.FromResult(model);
        }

        private Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScriptTableModel>(1, nameof(ScriptTableModel.TableName), ColumnType.Link),
                ColumnConfig.Create<ScriptTableModel>(2, nameof(ScriptTableModel.ScriptGroupName), hidden: true),
                ColumnConfig.Create<ScriptTableModel>(2, nameof(ScriptTableModel.Order))
            };

            return Task.FromResult(columns);
        }

        public virtual async Task<ScriptTableFormModel> PrepareScriptTableFormModelAsync(ScriptTableFormModel formModel, int traderId)
        {
            var scriptGroups = (await _scriptGroupService.GetAllScriptGroupsAsync(traderId))
                .OrderBy(x => x.GroupName)
                .Select(x => new SelectionItemList(x.Id, x.GroupName)).ToList();

            var tableNames = (await _scriptTableNameService.GetAllScriptTableNamesAsync())
                .Select(x => new SelectionList(x.Name, x.Name)).ToList();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptTableModel>(nameof(ScriptTableModel.TableName), FieldType.Autocomplete, required: true, options: tableNames),
                //FieldConfig.Create<ScriptTableModel>(nameof(ScriptTableModel.Group), FieldType.Text),
                FieldConfig.Create<ScriptTableModel>(nameof(ScriptTableModel.ScriptGroupId), FieldType.GridSelect, options: scriptGroups),
                FieldConfig.Create<ScriptTableModel>(nameof(ScriptTableModel.Order), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptTableModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}