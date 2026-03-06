using App.Core;
using App.Core.Domain.Offices;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Offices;
using App.Services;
using App.Services.Localization;
using App.Services.Offices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Offices
{
    public partial interface IAccountingOfficeModelFactory
    {
        Task<AccountingOfficeModel> PrepareAccountingOfficeModelAsync(AccountingOfficeModel model, AccountingOffice accountingOffice);
        Task<AccountingOfficeFormModel> PrepareAccountingOfficeFormModelAsync(AccountingOfficeFormModel formModel);
    }
    public partial class AccountingOfficeModelFactory : IAccountingOfficeModelFactory
    {
        private readonly IAccountingOfficeService _accountingOfficeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public AccountingOfficeModelFactory(IAccountingOfficeService accountingOfficeService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _accountingOfficeService = accountingOfficeService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual Task<AccountingOfficeModel> PrepareAccountingOfficeModelAsync(AccountingOfficeModel model, AccountingOffice accountingOffice)
        {
            if (accountingOffice != null)
            {
                //fill in model values from the entity
                model ??= accountingOffice.ToModel<AccountingOfficeModel>();
            }

            return Task.FromResult(model);
        }

        public virtual async Task<AccountingOfficeFormModel> PrepareAccountingOfficeFormModelAsync(AccountingOfficeFormModel formModel)
        {
            var legalStatusTypes = await LegalStatusType.AccountingFirmLegalPerson.ToSelectionItemListAsync();

            var aboutPanel1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.Vat), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.LastName), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.FirstName), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.Address), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.City), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.ZipPostalCode), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.PhoneNumber), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.Doy), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.Am), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.LicenseCategory), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.Adt), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.LegalStatusTypeId), FieldType.Select, options: legalStatusTypes),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.Comment), FieldType.Textarea)
            };

            var aboutPanel2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.TaxisNetUserName), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.TaxisNetPassword), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.AadeRegistryUsername), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.AadeRegistryPassword), FieldType.Text)
            };

            var officePanel = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.OfficeDataBaseName), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.OfficeUsername), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.OfficePassword), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.OfficeIpAddress), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.OfficePort), FieldType.Text)
            };

            var taxSystemPanel = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.TaxSystemDataBaseName), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.TaxSystemUsername), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.TaxSystemPassword), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.TaxSystemIpAddress), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.TaxSystemPort), FieldType.Text)
            };

            var hyperPayrollPanel = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.HyperPayrollDataBaseName), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.HyperPayrollUsername), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.HyperPayrollPassword), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.HyperPayrollIpAddress), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.HyperPayrollPort), FieldType.Text)
            };

            var hyperLogPanel = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.HyperLogDataBaseName), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.HyperLogUsername), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.HyperLogPassword), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.HyperLogIpAddress), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.HyperLogPort), FieldType.Text)
            };

            var srfPanel = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.SrfDataBaseName), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.SrfUsername), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.SrfPassword), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.SrfIpAddress), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.SrfPort), FieldType.Text)
            };

            var prosvasisPanel = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.ProsvasisDataBaseName), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.ProsvasisUsername), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.ProsvasisPassword), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.ProsvasisIpAddress), FieldType.Text),
                FieldConfig.Create<AccountingOfficeModel>(nameof(AccountingOfficeModel.ProsvasisPort), FieldType.Text)
            };

            var panels = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.AccountingOfficeModel.Panels.About"), true, "col-12 md:col-6", aboutPanel1, aboutPanel2),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.AccountingOfficeModel.Panels.Office"), false, "col-12 md:col-6", officePanel),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.AccountingOfficeModel.Panels.TaxSystem"), false, "col-12 md:col-6", taxSystemPanel),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.AccountingOfficeModel.Panels.HyperPayroll"), false, "col-12 md:col-6", hyperPayrollPanel),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.AccountingOfficeModel.Panels.HyperLog"), false, "col-12 md:col-6", hyperLogPanel),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.AccountingOfficeModel.Panels.Srf"), false, "col-12 md:col-6", srfPanel),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.AccountingOfficeModel.Panels.Prosvasis"), false, "col-12 md:col-6", prosvasisPanel)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.AccountingOfficeModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", panels);

            return formModel;
        }
    }
}