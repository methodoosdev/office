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
using Humanizer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.VatExemption
{
    public partial interface IVatExemptionDocModelFactory
    {
        Task<VatExemptionDocSearchModel> PrepareVatExemptionDocSearchModelAsync(VatExemptionDocSearchModel searchModel);
        Task<VatExemptionDocListModel> PrepareVatExemptionDocListModelAsync(VatExemptionDocSearchModel searchModel, Trader trader);
        Task<VatExemptionDocModel> PrepareVatExemptionDocModelAsync(VatExemptionDocModel model, VatExemptionDoc vatExemptionDoc, Trader trader, VatExemptionSerial vatExemptionSerial);
        Task<VatExemptionDocModel> PrepareVatExemptionDocSerialChangedAsync(VatExemptionDocModel model);
        Task<VatExemptionDocModel> PrepareVatExemptionDocChangedAsync(VatExemptionDocModel model);
        Task<VatExemptionDocFormModel> PrepareVatExemptionDocFormModelAsync(VatExemptionDocFormModel formModel, bool editMode, int traderId, int vatExemptionApprovalId);
    }
    public partial class VatExemptionDocModelFactory : IVatExemptionDocModelFactory
    {
        private readonly IVatExemptionDocService _vatExemptionDocService;
        private readonly IVatExemptionSerialService _vatExemptionSerialService;
        private readonly IVatExemptionApprovalService _vatExemptionApprovalService;
        private readonly IPersistStateService _persistStateService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public VatExemptionDocModelFactory(IVatExemptionDocService vatExemptionDocService,
            IVatExemptionSerialService vatExemptionSerialService,
            IVatExemptionApprovalService vatExemptionApprovalService,
            IPersistStateService persistStateService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _vatExemptionDocService = vatExemptionDocService;
            _vatExemptionSerialService = vatExemptionSerialService;
            _vatExemptionApprovalService = vatExemptionApprovalService;
            _persistStateService = persistStateService;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        private async Task<decimal> GetVatExemptionSerialLimitBalanceAsync(int vatExemptionSerialId)
        {
            var lastVatExemptionDoc = _vatExemptionDocService.Table
                .Where(x => x.VatExemptionSerialId == vatExemptionSerialId)
                .OrderByDescending(o => o.SerialNo)
                .FirstOrDefault();

            if (lastVatExemptionDoc == null)
            {
                var vatExemptionSerial = await _vatExemptionSerialService.GetVatExemptionSerialByIdAsync(vatExemptionSerialId);
                return vatExemptionSerial?.Limit ?? 0m;
            }

            return lastVatExemptionDoc.CurrentLimit;
        }

        private async Task<string> ConvertDecimalToWordsAsync(decimal value)
        {
            value = Math.Abs(value);
            var language = await _workContext.GetWorkingLanguageAsync();
            var cultureInfo = new CultureInfo(language.UniqueSeoCode);
            var _integerPart = Math.Truncate(value);
            var _decimalPart = (value - _integerPart) * 100;

            var integerPart = decimal.ToInt32(_integerPart);
            var decimalPart = decimal.ToInt32(_decimalPart);

            var leftPart = integerPart.ToWords(false, cultureInfo);
            leftPart += cultureInfo.Name == "el" ? " ευρώ" : " euro";
            var rightPart = "";
            if (decimalPart > 0)
            {
                rightPart = cultureInfo.Name == "el" ? " και " : " and ";
                rightPart += decimalPart.ToWords(false, cultureInfo);
                rightPart += cultureInfo.Name == "el" ? " λεπτά" : " minutes";
            }

            var final = leftPart + rightPart + ".";

            return final.ToPascalCase();
        }

        private async Task<IPagedList<VatExemptionDocModel>> GetPagedListAsync(VatExemptionDocSearchModel searchModel, Trader trader)
        {
            var query = _vatExemptionDocService.Table
                .Where(w => w.TraderId == trader.Id)
                .SelectAwait(async x =>
                {
                    var model = x.ToModel<VatExemptionDocModel>();

                    if (x.CreatedDate.HasValue)
                        model.CreatedDateValue = (await _dateTimeHelper.ConvertToUserTimeAsync(x.CreatedDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yyyy");

                    model.TraderName = trader.ToTraderFullName();

                    return model;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.ApprovalNumber.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<VatExemptionDocSearchModel> PrepareVatExemptionDocSearchModelAsync(VatExemptionDocSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<VatExemptionDocSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.VatExemptionDocModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<VatExemptionDocListModel> PrepareVatExemptionDocListModelAsync(VatExemptionDocSearchModel searchModel, Trader trader)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var vatExemptionDocs = await GetPagedListAsync(searchModel, trader);

            //prepare grid model
            var model = new VatExemptionDocListModel().PrepareToGrid(searchModel, vatExemptionDocs);

            return model;
        }

        public virtual async Task<VatExemptionDocModel> PrepareVatExemptionDocModelAsync(VatExemptionDocModel model, VatExemptionDoc vatExemptionDoc, Trader _trader, VatExemptionSerial vatExemptionSerial)
        {
            if (vatExemptionDoc != null)
            {
                //fill in model values from the entity
                model ??= vatExemptionDoc.ToModel<VatExemptionDocModel>();
            }

            if (vatExemptionDoc == null)
            {
                var vatExemptionApproval = await _vatExemptionApprovalService.GetVatExemptionApprovalByIdAsync(vatExemptionSerial.VatExemptionApprovalId);
                var limitBalance = await GetVatExemptionSerialLimitBalanceAsync(vatExemptionSerial.Id);
                var trader = _trader.ToModel<TraderModel>();


                model.VatExemptionSerialId = vatExemptionSerial.Id;
                model.TraderId = trader.Id;

                model.ApprovalLimit = vatExemptionApproval.Limit;
                model.SerialNo = vatExemptionSerial.SerialNo + 1;
                model.SerialLimit = vatExemptionSerial.Limit;
                model.SerialName = vatExemptionSerial.SerialName;
                model.ApprovalNumber = vatExemptionApproval.ApprovalNumber;
                model.ApprovalExpiryDate = vatExemptionApproval.ExpiryDate.Value;
                model.TraderFullName = trader.FullName();
                model.TraderProfessionalActivity = trader.ProfessionalActivity;
                model.TraderAddress = trader.JobAddress;
                model.TraderStreetNumber = trader.JobStreetNumber;
                model.TraderPostcode = trader.JobPostcode;
                model.TraderCity = trader.JobCity;
                model.TraderVat = trader.Vat;
                model.TraderDoy = trader.Doy;
                model.LimitBalance = limitBalance;
                model.AdjustedLimit = limitBalance;
                model.CurrentLimit = limitBalance;
                model.CurrentLimitAlphabet = await ConvertDecimalToWordsAsync(limitBalance);

                model.DocumentCity = trader.JobCity;
                model.CreatedDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Utc);
            }

            return model;
        }

        public virtual async Task<VatExemptionDocModel> PrepareVatExemptionDocSerialChangedAsync(VatExemptionDocModel model)
        {
            var vatExemptionSerial = await _vatExemptionSerialService.GetVatExemptionSerialByIdAsync(model.VatExemptionSerialId);

            var limitBalance = await GetVatExemptionSerialLimitBalanceAsync(vatExemptionSerial.Id);

            model.SerialNo = vatExemptionSerial.SerialNo + 1;
            model.SerialLimit = vatExemptionSerial.Limit;
            model.SerialName = vatExemptionSerial.SerialName;

            model.LimitBalance = limitBalance;
            model.AdjustedLimit = limitBalance;
            model.CurrentLimit = limitBalance;
            model.CurrentLimitAlphabet = await ConvertDecimalToWordsAsync(limitBalance);

            model.ReturnDiscount = 0m;
            model.TransferFromSeries = 0m;
            model.TransferToSeries = 0m;
            model.CurrentTransaction = 0m;
            model.CurrentTransactionAlphabet = "";

            return model;
        }

        public virtual async Task<VatExemptionDocModel> PrepareVatExemptionDocChangedAsync(VatExemptionDocModel model)
        {
            model.AdjustedLimit = model.LimitBalance + model.ReturnDiscount + model.TransferFromSeries - model.TransferToSeries;
            model.CurrentTransactionAlphabet = await ConvertDecimalToWordsAsync(model.CurrentTransaction);
            model.CurrentLimit = model.AdjustedLimit - model.CurrentTransaction;
            model.CurrentLimitAlphabet = await ConvertDecimalToWordsAsync(model.CurrentLimit);

            return model;
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var headerAlign = new Dictionary<string, string> { ["justify-content"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<VatExemptionDocModel>(1, nameof(VatExemptionDocModel.ApprovalNumber), hidden: true),
                ColumnConfig.Create<VatExemptionDocModel>(2, nameof(VatExemptionDocModel.ApprovalLimit), ColumnType.Decimal, style: rightAlign, hidden: true),
                ColumnConfig.Create<VatExemptionDocModel>(3, nameof(VatExemptionDocModel.ApprovalExpiryDate), ColumnType.Date, hidden: true),
                ColumnConfig.Create<VatExemptionDocModel>(4, nameof(VatExemptionDocModel.SerialNo), ColumnType.RouterLink),
                ColumnConfig.Create<VatExemptionDocModel>(3, nameof(VatExemptionDocModel.CreatedDate), ColumnType.Date),
                ColumnConfig.Create<VatExemptionDocModel>(5, nameof(VatExemptionDocModel.SerialName)),
                ColumnConfig.Create<VatExemptionDocModel>(6, nameof(VatExemptionDocModel.SerialLimit), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<VatExemptionDocModel>(7, nameof(VatExemptionDocModel.CurrentTransaction), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<VatExemptionDocModel>(8, nameof(VatExemptionDocModel.CurrentLimit), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<VatExemptionDocModel>(9, nameof(VatExemptionDocModel.Customs)),
                ColumnConfig.Create<VatExemptionDocModel>(9, nameof(VatExemptionDocModel.SupplierFullName)),
                ColumnConfig.Create<VatExemptionDocModel>(9, nameof(VatExemptionDocModel.SupplierVat)),
                ColumnConfig.Create<VatExemptionDocModel>(10, nameof(VatExemptionDocModel.LimitBalance), ColumnType.Decimal, style: rightAlign, hidden: true),
                ColumnConfig.Create<VatExemptionDocModel>(11, nameof(VatExemptionDocModel.ReturnDiscount), ColumnType.Decimal, style: rightAlign, hidden: true),
                ColumnConfig.Create<VatExemptionDocModel>(12, nameof(VatExemptionDocModel.AdjustedLimit), ColumnType.Decimal, style: rightAlign, hidden: true),
                ColumnConfig.CreateButton<VatExemptionApprovalModel>(13, ColumnType.RowButton, "exportToPdf", "warning",
                    await _localizationService.GetResourceAsync("App.Common.Print"), textAlign, headerAlign)
            };

            return columns;
        }

        private async Task<List<SelectionItemList>> GetVatExemptionSerialsAsync(int traderId, int vatExemptionApprovalId)
        {
            var items = (await _vatExemptionSerialService.GetAllVatExemptionSerialsAsync(traderId))
                .Where(w => w.VatExemptionApprovalId == vatExemptionApprovalId)
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.SerialName }).ToList();

            return items;
        }

        public virtual async Task<VatExemptionDocFormModel> PrepareVatExemptionDocFormModelAsync(VatExemptionDocFormModel formModel, bool editMode, int traderId, int vatExemptionApprovalId)
        {
            var serials = await GetVatExemptionSerialsAsync(traderId, vatExemptionApprovalId);

            var about = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.ApprovalLimit), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.VatExemptionSerialId), FieldType.Select, options: serials, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.SerialNo), FieldType.Text, _readonly: true),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.SerialLimit), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.ApprovalNumber), FieldType.Text, _readonly: true),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.ApprovalExpiryDate), FieldType.Date, _readonly: true)
            };

            var empty = new List<Dictionary<string, object>>();

            var traderItems1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.TraderFullName), FieldType.Text, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.TraderProfessionalActivity), FieldType.Text, _readonly: editMode)
            };

            var traderItems2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.TraderAddress), FieldType.Text, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.TraderPostcode), FieldType.Text, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.TraderVat), FieldType.Text, _readonly: editMode)
            };

            var traderItems3 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.TraderStreetNumber), FieldType.Text, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.TraderCity), FieldType.Text, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.TraderDoy), FieldType.Text, _readonly: editMode)
            };

            var supplierItems1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.SupplierFullName), FieldType.Text, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.SupplierProfessionalActivity), FieldType.Text, _readonly: editMode)
            };

            var supplierItems2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.SupplierAddress), FieldType.Text, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.SupplierPostcode), FieldType.Text, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.SupplierVat), FieldType.TextButton, _readonly: editMode)
            };

            var supplierItems3 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.SupplierStreetNumber), FieldType.Text, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.SupplierCity), FieldType.Text, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.SupplierDoy), FieldType.Text, _readonly: editMode)
            };

            var customs = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.Customs), FieldType.Textarea, _readonly: editMode)
            };

            var limitItems = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.LimitBalance), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.ReturnDiscount), FieldType.Decimals, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.TransferFromSeries), FieldType.Decimals, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.TransferToSeries), FieldType.Decimals, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.AdjustedLimit), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.CurrentTransaction), FieldType.Decimals, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.CurrentTransactionAlphabet), FieldType.Textarea, _readonly: editMode),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.CurrentLimit), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.CurrentLimitAlphabet), FieldType.Textarea, _readonly: editMode)
            };

            var documentCity = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.DocumentCity), FieldType.Text, _readonly: editMode)
            };

            var createdDate = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatExemptionDocModel>(nameof(VatExemptionDocModel.CreatedDate), FieldType.Date, _readonly: editMode)
            };

            var title1 = await _localizationService.GetResourceAsync("App.Models.VatExemptionDocModel.Panels.Title1");
            var title2 = await _localizationService.GetResourceAsync("App.Models.VatExemptionDocModel.Panels.Title2");
            var panels = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreatePanel(title1 + " - " + title2, true, "col-12 md:col-6", about),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.VatExemptionDocModel.Panels.TraderItems"), true, "col-12 md:col-6", traderItems1, empty, traderItems2, traderItems3),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.VatExemptionDocModel.Panels.SupplierItems"), true, "col-12 md:col-6", supplierItems1, empty, supplierItems2, supplierItems3, customs),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.VatExemptionDocModel.Panels.LimitItems"), true, "col-12 md:col-6", limitItems, empty, empty, empty, documentCity, createdDate)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.VatExemptionDocModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", panels);

            return formModel;
        }
    }
}