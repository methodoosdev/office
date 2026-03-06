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
using App.Services.Localization;
using App.Services.Offices;
using App.Services.VatExemption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.VatExemption
{
    public partial interface IVatExemptionSerialModelFactory
    {
        Task<VatExemptionSerialSearchModel> PrepareVatExemptionSerialSearchModelAsync(VatExemptionSerialSearchModel searchModel);
        Task<VatExemptionSerialListModel> PrepareVatExemptionSerialListModelAsync(VatExemptionSerialSearchModel searchModel, Trader trader);
        Task<VatExemptionSerialModel> PrepareVatExemptionSerialModelAsync(VatExemptionSerialModel model, VatExemptionSerial vatExemptionSerial, int traderId);
        Task<VatExemptionSerialFormModel> PrepareVatExemptionSerialFormModelAsync(VatExemptionSerialFormModel formModel, int traderId, bool readOnlyMode);
    }
    public partial class VatExemptionSerialModelFactory : IVatExemptionSerialModelFactory
    {
        private readonly IVatExemptionSerialService _vatExemptionSerialService;
        private readonly IVatExemptionApprovalService _vatExemptionApprovalService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IPersistStateService _persistStateService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public VatExemptionSerialModelFactory(IVatExemptionSerialService vatExemptionSerialService,
            IVatExemptionApprovalService vatExemptionApprovalService,
            IModelFactoryService modelFactoryService,
            IPersistStateService persistStateService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _vatExemptionSerialService = vatExemptionSerialService;
            _vatExemptionApprovalService = vatExemptionApprovalService;
            _modelFactoryService = modelFactoryService;
            _persistStateService = persistStateService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<VatExemptionSerialModel>> GetPagedListAsync(VatExemptionSerialSearchModel searchModel, Trader trader)
        {
            var query = _vatExemptionSerialService.Table
                .Where(w => w.TraderId == trader.Id)
                .SelectAwait(async x =>
                {
                    var model = x.ToModel<VatExemptionSerialModel>();
                    var vatExemptionApproval = await _vatExemptionApprovalService.GetVatExemptionApprovalByIdAsync(x.VatExemptionApprovalId);

                    model.TraderName = trader.ToTraderFullName();
                    model.ApprovalNumber = vatExemptionApproval?.ApprovalNumber ?? "";
                    model.ApprovalLimit = vatExemptionApproval?.Limit ?? 0;
                    model.ApprovalActive = vatExemptionApproval?.Active ?? false;

                    return model;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<VatExemptionSerialSearchModel> PrepareVatExemptionSerialSearchModelAsync(VatExemptionSerialSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<VatExemptionSerialSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.VatExemptionSerialModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<VatExemptionSerialListModel> PrepareVatExemptionSerialListModelAsync(VatExemptionSerialSearchModel searchModel, Trader trader)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var vatExemptionSerials = await GetPagedListAsync(searchModel, trader);

            //prepare grid model
            var model = new VatExemptionSerialListModel().PrepareToGrid(searchModel, vatExemptionSerials);

            return model;
        }

        public virtual Task<VatExemptionSerialModel> PrepareVatExemptionSerialModelAsync(VatExemptionSerialModel model, VatExemptionSerial vatExemptionSerial, int traderId)
        {
            if (vatExemptionSerial != null)
            {
                //fill in model values from the entity
                model ??= vatExemptionSerial.ToModel<VatExemptionSerialModel>();
            }

            if (vatExemptionSerial == null)
            {
                model.TraderId = traderId;
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<VatExemptionSerialModel>(1, nameof(VatExemptionSerialModel.ApprovalNumber)),
                ColumnConfig.Create<VatExemptionSerialModel>(2, nameof(VatExemptionSerialModel.ApprovalLimit), ColumnType.Decimal),
                ColumnConfig.Create<VatExemptionSerialModel>(3, nameof(VatExemptionSerialModel.ApprovalActive), ColumnType.Checkbox),
                ColumnConfig.Create<VatExemptionSerialModel>(4, nameof(VatExemptionSerialModel.SerialNo), ColumnType.Numeric),
                ColumnConfig.Create<VatExemptionSerialModel>(5, nameof(VatExemptionSerialModel.SerialName), ColumnType.RouterLink),
                ColumnConfig.Create<VatExemptionSerialModel>(6, nameof(VatExemptionSerialModel.Limit), ColumnType.Decimal),
                ColumnConfig.Create<VatExemptionSerialModel>(7, nameof(VatExemptionSerialModel.Manuscript), ColumnType.Checkbox)
            };

            return columns;
        }

        public virtual async Task<VatExemptionSerialFormModel> PrepareVatExemptionSerialFormModelAsync(VatExemptionSerialFormModel formModel, int traderId, bool readOnlyMode)
        {
            var vatExemptionApprovals = await _modelFactoryService.GetAllVatExemptionApprovalsAsync(traderId, false);

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionSerialModel>(nameof(VatExemptionSerialModel.VatExemptionApprovalId), FieldType.Select, markAsRequired: true, options: vatExemptionApprovals),
                FieldConfig.Create<VatExemptionSerialModel>(nameof(VatExemptionSerialModel.SerialNo), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<VatExemptionSerialModel>(nameof(VatExemptionSerialModel.SerialName), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<VatExemptionSerialModel>(nameof(VatExemptionSerialModel.SerialNameDescription), FieldType.Textarea),
                FieldConfig.Create<VatExemptionSerialModel>(nameof(VatExemptionSerialModel.Limit), FieldType.Decimals, markAsRequired: true, _readonly: readOnlyMode),
                FieldConfig.Create<VatExemptionSerialModel>(nameof(VatExemptionSerialModel.Manuscript), FieldType.Checkbox),
                FieldConfig.Create<VatExemptionSerialModel>(nameof(VatExemptionSerialModel.Description), FieldType.Textarea),
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.VatExemptionSerialModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}