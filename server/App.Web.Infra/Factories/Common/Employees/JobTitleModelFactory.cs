using App.Core;
using App.Core.Domain.Employees;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models;
using App.Framework.Models.Extensions;
using App.Models.Employees;
using App.Services.Employees;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Employees
{
    public partial interface IJobTitleModelFactory
    {
        Task<JobTitleSearchModel> PrepareJobTitleSearchModelAsync(JobTitleSearchModel searchModel);
        Task<JobTitleListModel> PrepareJobTitleListModelAsync(JobTitleSearchModel searchModel);
        Task<JobTitleModel> PrepareJobTitleModelAsync(JobTitleModel model, JobTitle jobTitle);
        Task<JobTitleFormModel> PrepareJobTitleFormModelAsync(JobTitleFormModel formModel);
    }
    public partial class JobTitleModelFactory : IJobTitleModelFactory
    {
        private readonly IJobTitleService _jobTitleService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public JobTitleModelFactory(IJobTitleService jobTitleService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _jobTitleService = jobTitleService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<JobTitleModel>> GetPagedListAsync(JobTitleSearchModel searchModel)
        {
            var query = _jobTitleService.Table.AsEnumerable()
                .Select(x => x.ToModel<JobTitleModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<JobTitleSearchModel> PrepareJobTitleSearchModelAsync(JobTitleSearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.JobTitleModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<JobTitleListModel> PrepareJobTitleListModelAsync(JobTitleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var jobTitles = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new JobTitleListModel().PrepareToGrid(searchModel, jobTitles);

            return model;
        }

        public virtual Task<JobTitleModel> PrepareJobTitleModelAsync(JobTitleModel model, JobTitle jobTitle)
        {
            if (jobTitle != null)
            {
                //fill in model values from the entity
                model ??= jobTitle.ToModel<JobTitleModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<JobTitleModel>(1, nameof(JobTitleModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<JobTitleModel>(2, nameof(JobTitleModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<JobTitleFormModel> PrepareJobTitleFormModelAsync(JobTitleFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<JobTitleModel>(nameof(JobTitleModel.Description), FieldType.Text),
                FieldConfig.Create<JobTitleModel>(nameof(JobTitleModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.JobTitleModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}