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
    public partial interface IVatExemptionApprovalModelFactory
    {
        Task<VatExemptionApprovalSearchModel> PrepareVatExemptionApprovalSearchModelAsync(VatExemptionApprovalSearchModel searchModel);
        Task<VatExemptionApprovalListModel> PrepareVatExemptionApprovalListModelAsync(VatExemptionApprovalSearchModel searchModel, Trader trader);
        Task<VatExemptionApprovalModel> PrepareVatExemptionApprovalModelAsync(VatExemptionApprovalModel model, VatExemptionApproval VatExemptionApproval, int traderId);
        Task<VatExemptionApprovalFormModel> PrepareVatExemptionApprovalFormModelAsync(VatExemptionApprovalFormModel formModel, bool readOnlyMode);
    }
    public partial class VatExemptionApprovalModelFactory : IVatExemptionApprovalModelFactory
    {
        private readonly IVatExemptionApprovalService _vatExemptionApprovalService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPersistStateService _persistStateService;
        private readonly IWorkContext _workContext;

        public VatExemptionApprovalModelFactory(
            IVatExemptionApprovalService vatExemptionApprovalService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IPersistStateService persistStateService,
            IWorkContext workContext)
        {
            _vatExemptionApprovalService = vatExemptionApprovalService;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _persistStateService = persistStateService;
            _workContext = workContext;
        }

        private async Task<IPagedList<VatExemptionApprovalModel>> GetPagedListAsync(VatExemptionApprovalSearchModel searchModel, Trader trader)
        {
            var query = _vatExemptionApprovalService.Table
                .Where(w => w.TraderId == trader.Id)
                .SelectAwait(async x =>
                {
                    var model = x.ToModel<VatExemptionApprovalModel>();

                    if (x.CreatedDate.HasValue)
                        model.CreatedDateValue = (await _dateTimeHelper.ConvertToUserTimeAsync(x.CreatedDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yyyy");

                    if (x.StartingDate.HasValue)
                        model.StartingDateValue = (await _dateTimeHelper.ConvertToUserTimeAsync(x.StartingDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yyyy");

                    if (x.ExpiryDate.HasValue)
                        model.ExpiryDateValue = (await _dateTimeHelper.ConvertToUserTimeAsync(x.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yyyy");

                    model.TraderName = trader.ToTraderFullName();

                    return model;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<VatExemptionApprovalSearchModel> PrepareVatExemptionApprovalSearchModelAsync(VatExemptionApprovalSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<VatExemptionApprovalSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            searchModel.SetGridPageSize();
            //prepare page parameters
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.VatExemptionApprovalModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<VatExemptionApprovalListModel> PrepareVatExemptionApprovalListModelAsync(VatExemptionApprovalSearchModel searchModel, Trader trader)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var VatExemptionApprovals = await GetPagedListAsync(searchModel, trader);

            //prepare grid model
            var model = new VatExemptionApprovalListModel().PrepareToGrid(searchModel, VatExemptionApprovals);

            return model;
        }

        public virtual Task<VatExemptionApprovalModel> PrepareVatExemptionApprovalModelAsync(VatExemptionApprovalModel model, VatExemptionApproval VatExemptionApproval, int traderId)
        {
            if (VatExemptionApproval != null)
            {
                //fill in model values from the entity
                model ??= VatExemptionApproval.ToModel<VatExemptionApprovalModel>();

                var item = new { Name = "First.txt", Size = 500 };
                model.KendoUpload = new List<object> { item };
            }

            if (VatExemptionApproval == null)
            {
                var date = DateTime.UtcNow;

                model.TraderId = traderId;
                model.CreatedDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                model.StartingDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                model.ExpiryDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            }

            return Task.FromResult(model);
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<VatExemptionApprovalModel>(1, nameof(VatExemptionApprovalModel.ExpiryDateValue)),
                ColumnConfig.Create<VatExemptionApprovalModel>(2, nameof(VatExemptionApprovalModel.ApprovalNumber), ColumnType.RouterLink),
                ColumnConfig.Create<VatExemptionApprovalModel>(3, nameof(VatExemptionApprovalModel.ApprovalProtocol)),
                ColumnConfig.Create<VatExemptionApprovalModel>(4, nameof(VatExemptionApprovalModel.Limit), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<VatExemptionApprovalModel>(5, nameof(VatExemptionApprovalModel.Doy), hidden: true),
                //ColumnConfig.Create<VatExemptionApprovalModel>(4, nameof(VatExemptionApprovalModel.FileName), ColumnType.Download),
                ColumnConfig.Create<VatExemptionApprovalModel>(6, nameof(VatExemptionApprovalModel.CreatedDateValue), hidden: true),
                ColumnConfig.Create<VatExemptionApprovalModel>(7, nameof(VatExemptionApprovalModel.StartingDateValue), hidden: true),
                ColumnConfig.Create<VatExemptionApprovalModel>(8, nameof(VatExemptionApprovalModel.Active), ColumnType.Checkbox),
                ColumnConfig.CreateButton<VatExemptionApprovalModel>(9, ColumnType.RowButton, "setPrimary", "info",
                    await _localizationService.GetResourceAsync("App.Common.Basic"), textAlign, textAlign)
            };

            return columns;
        }

        public virtual async Task<VatExemptionApprovalFormModel> PrepareVatExemptionApprovalFormModelAsync(VatExemptionApprovalFormModel formModel, bool readOnlyMode)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionApprovalModel>(nameof(VatExemptionApprovalModel.ApprovalNumber), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<VatExemptionApprovalModel>(nameof(VatExemptionApprovalModel.ApprovalProtocol), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<VatExemptionApprovalModel>(nameof(VatExemptionApprovalModel.Limit), FieldType.Decimals, markAsRequired: true, _readonly: readOnlyMode),
                FieldConfig.Create<VatExemptionApprovalModel>(nameof(VatExemptionApprovalModel.Doy), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<VatExemptionApprovalModel>(nameof(VatExemptionApprovalModel.StartingDate), FieldType.Date, markAsRequired: true),
                FieldConfig.Create<VatExemptionApprovalModel>(nameof(VatExemptionApprovalModel.ExpiryDate), FieldType.Date, markAsRequired: true),
                FieldConfig.Create<VatExemptionApprovalModel>(nameof(VatExemptionApprovalModel.Description), FieldType.Textarea),
                FieldConfig.Create<VatExemptionApprovalModel>(nameof(VatExemptionApprovalModel.CreatedDate), FieldType.Date, _readonly: true),
                //FieldConfig.Create<VatExemptionApprovalModel>(nameof(VatExemptionApprovalModel.FileName), FieldType.Text),
                //FieldConfig.Create<VatExemptionApprovalModel>(nameof(VatExemptionApprovalModel.KendoUpload), FieldType.Upload, className: "col-12 k-upload-max-width")
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.VatExemptionApprovalModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}