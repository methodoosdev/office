using App.Core;
using App.Core.Domain.Traders;
using App.Core.Domain.VatExemption;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Common;
using App.Models.Traders;
using App.Models.VatExemption;
using App.Services;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.VatExemption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.VatExemption
{
    public partial interface IVatExemptionReportModelFactory
    {
        Task<VatExemptionReportSearchModel> PrepareVatExemptionReportSearchModelAsync(VatExemptionReportSearchModel searchModel);
        Task<VatExemptionReportListModel> PrepareVatExemptionReportListModelAsync(VatExemptionReportSearchModel searchModel, Trader trader);
        Task<VatExemptionReportModel> PrepareVatExemptionReportModelAsync(VatExemptionReportModel model, VatExemptionReport vatExemptionReport, int traderId);
        Task<VatExemptionReportFormModel> PrepareVatExemptionReportFormModelAsync(VatExemptionReportFormModel formModel, int traderId);
    }
    public partial class VatExemptionReportModelFactory : IVatExemptionReportModelFactory
    {
        private readonly IVatExemptionReportService _vatExemptionReportService;
        private readonly IVatExemptionApprovalService _vatExemptionApprovalService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IPersistStateService _persistStateService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public VatExemptionReportModelFactory(IVatExemptionReportService vatExemptionReportService,
            IVatExemptionApprovalService vatExemptionApprovalService,
            IModelFactoryService modelFactoryService,
            IPersistStateService persistStateService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _vatExemptionReportService = vatExemptionReportService;
            _vatExemptionApprovalService = vatExemptionApprovalService;
            _modelFactoryService = modelFactoryService;
            _persistStateService = persistStateService;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        private async Task<IPagedList<VatExemptionReportModel>> GetPagedListAsync(VatExemptionReportSearchModel searchModel, Trader trader)
        {
            var query = _vatExemptionReportService.Table
                .Where(w => w.TraderId == trader.Id)
                .SelectAwait(async x =>
                {
                    var model = x.ToModel<VatExemptionReportModel>();
                    var vatExemptionApproval = await _vatExemptionApprovalService.GetVatExemptionApprovalByIdAsync(x.VatExemptionApprovalId);

                    if (x.CreatedDate.HasValue)
                        model.CreatedDateValue = (await _dateTimeHelper.ConvertToUserTimeAsync(x.CreatedDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yyyy");

                    model.TraderName = trader.ToTraderFullName();
                    model.ApprovalNumber = vatExemptionApproval?.ApprovalNumber ?? "";
                    model.ApprovalLimit = vatExemptionApproval?.Limit ?? 0;
                    model.ApprovalDoy = vatExemptionApproval?.Doy ?? "";

                    return model;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<VatExemptionReportSearchModel> PrepareVatExemptionReportSearchModelAsync(VatExemptionReportSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<VatExemptionReportSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.VatExemptionReportModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<VatExemptionReportListModel> PrepareVatExemptionReportListModelAsync(VatExemptionReportSearchModel searchModel, Trader trader)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var vatExemptionReports = await GetPagedListAsync(searchModel, trader);

            //prepare grid model
            var model = new VatExemptionReportListModel().PrepareToGrid(searchModel, vatExemptionReports);

            return model;
        }

        public virtual Task<VatExemptionReportModel> PrepareVatExemptionReportModelAsync(VatExemptionReportModel model, VatExemptionReport vatExemptionReport, int traderId)
        {
            if (vatExemptionReport != null)
            {
                //fill in model values from the entity
                model ??= vatExemptionReport.ToModel<VatExemptionReportModel>();
            }

            if (vatExemptionReport == null)
            {
                var date = DateTime.UtcNow;

                model.TraderId = traderId;
                model.CreatedDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<VatExemptionReportModel>(1, nameof(VatExemptionReportModel.ApprovalNumber)),
                ColumnConfig.Create<VatExemptionReportModel>(2, nameof(VatExemptionReportModel.ApprovalLimit), ColumnType.Decimal),
                ColumnConfig.Create<VatExemptionReportModel>(3, nameof(VatExemptionReportModel.ApprovalDoy), hidden: true),
                ColumnConfig.Create<VatExemptionReportModel>(4, nameof(VatExemptionReportModel.Subject), ColumnType.RouterLink),
                ColumnConfig.Create<VatExemptionReportModel>(5, nameof(VatExemptionReportModel.Protocol)),
                //ColumnConfig.Create<VatExemptionReportModel>(6, nameof(VatExemptionReportModel.FileName)),
                ColumnConfig.Create<VatExemptionReportModel>(7, nameof(VatExemptionReportModel.CreatedDateValue))
            };

            return columns;
        }

        public virtual async Task<VatExemptionReportFormModel> PrepareVatExemptionReportFormModelAsync(VatExemptionReportFormModel formModel, int traderId)
        {
            var vatExemptionApprovals = await _modelFactoryService.GetAllVatExemptionApprovalsAsync(traderId, false);

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionReportModel>(nameof(VatExemptionReportModel.VatExemptionApprovalId), FieldType.Select, markAsRequired: true, options: vatExemptionApprovals),
                FieldConfig.Create<VatExemptionReportModel>(nameof(VatExemptionReportModel.Subject), FieldType.Textarea, markAsRequired: true),
                FieldConfig.Create<VatExemptionReportModel>(nameof(VatExemptionReportModel.Protocol), FieldType.Text),
                FieldConfig.Create<VatExemptionReportModel>(nameof(VatExemptionReportModel.Description), FieldType.Textarea, markAsRequired: true),
                //FieldConfig.Create<VatExemptionReportModel>(nameof(VatExemptionReportModel.FileName), FieldType.Text, _readonly: true),
                FieldConfig.Create<VatExemptionReportModel>(nameof(VatExemptionReportModel.CreatedDate), FieldType.Date, _readonly: true),
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.VatExemptionReportModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}