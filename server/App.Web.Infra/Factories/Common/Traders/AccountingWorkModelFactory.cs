using App.Core;
using App.Core.Domain.Traders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface IAccountingWorkModelFactory
    {
        Task<AccountingWorkSearchModel> PrepareAccountingWorkSearchModelAsync(AccountingWorkSearchModel searchModel);
        Task<AccountingWorkListModel> PrepareAccountingWorkListModelAsync(AccountingWorkSearchModel searchModel);
        Task<AccountingWorkModel> PrepareAccountingWorkModelAsync(AccountingWorkModel model, AccountingWork accountingWork);
        Task<AccountingWorkFormModel> PrepareAccountingWorkFormModelAsync(AccountingWorkFormModel formModel);
    }
    public partial class AccountingWorkModelFactory : IAccountingWorkModelFactory
    {
        private readonly IAccountingWorkService _accountingWorkService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public AccountingWorkModelFactory(IAccountingWorkService accountingWorkService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _accountingWorkService = accountingWorkService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<AccountingWorkSearchModel> PrepareAccountingWorkSearchModelAsync(AccountingWorkSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.AccountingWorkModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<AccountingWorkListModel> PrepareAccountingWorkListModelAsync(AccountingWorkSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var accountingWorks = await _accountingWorkService.GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new AccountingWorkListModel().PrepareToGrid(searchModel, accountingWorks);

            return model;
        }

        public virtual Task<AccountingWorkModel> PrepareAccountingWorkModelAsync(AccountingWorkModel model, AccountingWork accountingWork)
        {
            if (accountingWork != null)
            {
                //fill in model values from the entity
                model ??= accountingWork.ToModel<AccountingWorkModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AccountingWorkModel>(1, nameof(AccountingWorkModel.SortDescription), ColumnType.RouterLink),
                ColumnConfig.Create<AccountingWorkModel>(2, nameof(AccountingWorkModel.Price), ColumnType.Decimal),
                ColumnConfig.Create<AccountingWorkModel>(3, nameof(AccountingWorkModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<AccountingWorkFormModel> PrepareAccountingWorkFormModelAsync(AccountingWorkFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AccountingWorkModel>(nameof(AccountingWorkModel.SortDescription), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<AccountingWorkModel>(nameof(AccountingWorkModel.Description), FieldType.Textarea),
                FieldConfig.Create<AccountingWorkModel>(nameof(AccountingWorkModel.Price), FieldType.Numeric),
                FieldConfig.Create<AccountingWorkModel>(nameof(AccountingWorkModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.AccountingWorkModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}