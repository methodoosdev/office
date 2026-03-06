using App.Core;
using App.Core.Domain.Employees;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Employees;
using App.Services.Employees;
using App.Services.Localization;
using App.Web.Framework.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Employees
{
    public partial interface IEducationModelFactory
    {
        Task<EducationSearchModel> PrepareEducationSearchModelAsync(EducationSearchModel searchModel);
        Task<EducationListModel> PrepareEducationListModelAsync(EducationSearchModel searchModel);
        Task<EducationModel> PrepareEducationModelAsync(EducationModel model, Education education, bool excludeProperties = false);
        Task<EducationFormModel> PrepareEducationFormModelAsync(EducationFormModel formModel);
    }
    public partial class EducationModelFactory : IEducationModelFactory
    {
        private readonly IEducationService _educationService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;

        public EducationModelFactory(IEducationService educationService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            ILanguageService languageService,
            IWorkContext workContext)
        {
            _educationService = educationService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _languageService = languageService;
            _workContext = workContext;
        }

        private async Task<IPagedList<EducationModel>> GetPagedListAsync(EducationSearchModel searchModel)
        {
            var query = _educationService.Table.AsEnumerable()
                .Select(x => x.ToModel<EducationModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<EducationSearchModel> PrepareEducationSearchModelAsync(EducationSearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.EducationModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<EducationListModel> PrepareEducationListModelAsync(EducationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var educations = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new EducationListModel().PrepareToGrid(searchModel, educations);

            return model;
        }

        public virtual async Task<EducationModel> PrepareEducationModelAsync(EducationModel model, Education education, bool excludeProperties = false)
        {
            Func<EducationLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (education != null)
            {
                //fill in model values from the entity
                model ??= education.ToModel<EducationModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Description = await _localizationService.GetLocalizedAsync(education, entity => entity.Description, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<EducationModel>(1, nameof(EducationModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<EducationModel>(2, nameof(EducationModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<EducationFormModel> PrepareEducationFormModelAsync(EducationFormModel formModel)
        {
            //get all available languages
            var availableLanguages = await _languageService.GetAllLanguagesAsync(true);

            var locales = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EducationModel>(nameof(EducationModel.Description), FieldType.Text)
            };

            var fieldGroup = new Dictionary<string, object>
            {
                ["fieldGroupClassName"] = "grid",
                ["className"] = "col-12",
                ["fieldGroup"] = locales
            };

            var tabs = FieldConfig.Create<EducationModel>(nameof(EducationModel.Locales), FieldType.LocaleTabs);
            tabs.Add("fieldArray", fieldGroup);
            tabs.Add("wrappers", new List<string> { "simple-section" });

            var fields = new List<Dictionary<string, object>>()
            {
                tabs,
                FieldConfig.Create<EducationModel>(nameof(EducationModel.Description), FieldType.Text),
                FieldConfig.Create<EducationModel>(nameof(EducationModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.EducationModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}