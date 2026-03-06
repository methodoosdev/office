using App.Core.Domain.Localization;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models;
using App.Framework.Models.Extensions;
using App.Models.Localization;
using App.Services;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Admin.Factories
{
    public partial interface ILanguageModelFactory
    {
        Task<LanguageSearchModel> PrepareLanguageSearchModelAsync(LanguageSearchModel searchModel);
        Task<LanguageListModel> PrepareLanguageListModelAsync(LanguageSearchModel searchModel);
        Task<LanguageModel> PrepareLanguageModelAsync(LanguageModel model, Language language, bool excludeProperties = false);
        Task<LanguageFormModel> PrepareLanguageFormModelAsync(LanguageFormModel formModel, LanguageModel model);

    }
    public partial class LanguageModelFactory : ILanguageModelFactory
    {
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;

        public LanguageModelFactory(
            IModelFactoryService modelFactoryService,
            ILanguageService languageService,
            ILocalizationService localizationService)
        {
            _modelFactoryService = modelFactoryService;
            _languageService = languageService;
            _localizationService = localizationService;
        }

        public virtual async Task<LanguageSearchModel> PrepareLanguageSearchModelAsync(LanguageSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.LanguageModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<LanguageListModel> PrepareLanguageListModelAsync(LanguageSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get languages
            var languages = (await _languageService.GetAllLanguagesAsync(showHidden: true)).ToPagedList(searchModel);

            //prepare list model
            var model = new LanguageListModel().PrepareToGrid(searchModel, languages, () =>
            {
                return languages.Select(language => language.ToModel<LanguageModel>());
            });

            return model;
        }

        public virtual async Task<LanguageModel> PrepareLanguageModelAsync(LanguageModel model, Language language, bool excludeProperties = false)
        {
            if (language != null)
            {
                //fill in model values from the entity
                model ??= language.ToModel<LanguageModel>();
            }

            //set default values for the new model
            if (language == null)
            {
                model.DisplayOrder = (await _languageService.GetAllLanguagesAsync()).Max(l => l.DisplayOrder) + 1;
                model.Published = true;
            }

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<LanguageModel>(1, nameof(LanguageModel.Name), ColumnType.RouterLink),
                ColumnConfig.Create<LanguageModel>(2, nameof(LanguageModel.LanguageCulture)),
                ColumnConfig.Create<LanguageModel>(2, nameof(LanguageModel.UniqueSeoCode)),
                ColumnConfig.Create<LanguageModel>(2, nameof(LanguageModel.FlagImageFileName)),
                ColumnConfig.Create<LanguageModel>(2, nameof(LanguageModel.Published), ColumnType.Checkbox),
                ColumnConfig.Create<LanguageModel>(2, nameof(LanguageModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<LanguageFormModel> PrepareLanguageFormModelAsync(LanguageFormModel formModel, LanguageModel model)
        {
            var flagFileNames = await _modelFactoryService.GetAllFlagFileNamesAsync();
            var currencies = await _modelFactoryService.GetAllCurrenciesAsync();
            var cultures = await _modelFactoryService.GetAllCulturesAsync();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<LanguageModel>(nameof(LanguageModel.Name), FieldType.Text),
                FieldConfig.Create<LanguageModel>(nameof(LanguageModel.LanguageCulture), FieldType.Select, options: cultures),
                FieldConfig.Create<LanguageModel>(nameof(LanguageModel.UniqueSeoCode), FieldType.Text),
                FieldConfig.Create<LanguageModel>(nameof(LanguageModel.DefaultCurrencyId), FieldType.Select, options: currencies),
                FieldConfig.Create<LanguageModel>(nameof(LanguageModel.FlagImageFileName), FieldType.Select, options: flagFileNames),
                FieldConfig.Create<LanguageModel>(nameof(LanguageModel.Published), FieldType.Checkbox),
                FieldConfig.Create<LanguageModel>(nameof(LanguageModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.LanguageModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }

    }
}