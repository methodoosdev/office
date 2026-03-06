using App.Core;
using App.Core.Domain.Assignment;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Assignment;
using App.Services.Assignment;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Assignment
{
    public partial interface IAssignmentReasonModelFactory
    {
        Task<AssignmentReasonSearchModel> PrepareAssignmentReasonSearchModelAsync(AssignmentReasonSearchModel searchModel);
        Task<AssignmentReasonListModel> PrepareAssignmentReasonListModelAsync(AssignmentReasonSearchModel searchModel);
        Task<AssignmentReasonModel> PrepareAssignmentReasonModelAsync(AssignmentReasonModel model, AssignmentReason assignmentReason);
        Task<AssignmentReasonFormModel> PrepareAssignmentReasonFormModelAsync(AssignmentReasonFormModel formModel);
    }
    public partial class AssignmentReasonModelFactory : IAssignmentReasonModelFactory
    {
        private readonly IAssignmentReasonService _assignmentReasonService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public AssignmentReasonModelFactory(IAssignmentReasonService assignmentReasonService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _assignmentReasonService = assignmentReasonService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<AssignmentReasonModel>> GetPagedListAsync(AssignmentReasonSearchModel searchModel)
        {
            var query = _assignmentReasonService.Table.AsEnumerable()
                .Select(x => x.ToModel<AssignmentReasonModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<AssignmentReasonSearchModel> PrepareAssignmentReasonSearchModelAsync(AssignmentReasonSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.AssignmentReasonModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<AssignmentReasonListModel> PrepareAssignmentReasonListModelAsync(AssignmentReasonSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var assignmentReasons = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new AssignmentReasonListModel().PrepareToGrid(searchModel, assignmentReasons);

            return model;
        }

        public virtual Task<AssignmentReasonModel> PrepareAssignmentReasonModelAsync(AssignmentReasonModel model, AssignmentReason assignmentReason)
        {
            if (assignmentReason != null)
            {
                //fill in model values from the entity
                model ??= assignmentReason.ToModel<AssignmentReasonModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AssignmentReasonModel>(1, nameof(AssignmentReasonModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<AssignmentReasonModel>(2, nameof(AssignmentReasonModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<AssignmentReasonFormModel> PrepareAssignmentReasonFormModelAsync(AssignmentReasonFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentReasonModel>(nameof(AssignmentReasonModel.Description), FieldType.Text),
                FieldConfig.Create<AssignmentReasonModel>(nameof(AssignmentReasonModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.AssignmentReasonModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}