using App.Core;
using App.Core.Domain.Scripts;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Scripts;
using App.Services.Localization;
using App.Services.Scripts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptTableNameModelFactory
    {
        Task<ScriptTableNameSearchModel> PrepareScriptTableNameSearchModelAsync(ScriptTableNameSearchModel searchModel);
        Task<ScriptTableNameListModel> PrepareScriptTableNameListModelAsync(ScriptTableNameSearchModel searchModel);
        Task<ScriptTableNameModel> PrepareScriptTableNameModelAsync(ScriptTableNameModel model, ScriptTableName scriptTableName);
        Task<ScriptTableNameFormModel> PrepareScriptTableNameFormModelAsync(ScriptTableNameFormModel formModel);
    }
    public partial class ScriptTableNameModelFactory : IScriptTableNameModelFactory
    {
        private readonly IScriptTableNameService _scriptTableNameService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptTableNameModelFactory(IScriptTableNameService scriptTableNameService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _scriptTableNameService = scriptTableNameService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<ScriptTableNameSearchModel> PrepareScriptTableNameSearchModelAsync(ScriptTableNameSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ScriptTableNameModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ScriptTableNameListModel> PrepareScriptTableNameListModelAsync(ScriptTableNameSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var scriptTableNames = await _scriptTableNameService.GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new ScriptTableNameListModel().PrepareToGrid(searchModel, scriptTableNames);

            return model;
        }

        public virtual Task<ScriptTableNameModel> PrepareScriptTableNameModelAsync(ScriptTableNameModel model, ScriptTableName scriptTableName)
        {
            if (scriptTableName != null)
            {
                //fill in model values from the entity
                model ??= scriptTableName.ToModel<ScriptTableNameModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ScriptTableNameModel>(1, nameof(ScriptTableNameModel.Name), ColumnType.RouterLink),
                ColumnConfig.Create<ScriptTableNameModel>(2, nameof(ScriptTableNameModel.Order))
            };

            return columns;
        }

        public virtual async Task<ScriptTableNameFormModel> PrepareScriptTableNameFormModelAsync(ScriptTableNameFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ScriptTableNameModel>(nameof(ScriptTableNameModel.Name), FieldType.Text),
                FieldConfig.Create<ScriptTableNameModel>(nameof(ScriptTableNameModel.Order), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ScriptTableNameModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}