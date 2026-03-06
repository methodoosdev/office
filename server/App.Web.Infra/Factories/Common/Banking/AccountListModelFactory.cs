using App.Core;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Models.Extensions;
using App.Models.Banking;
using App.Services.Banking;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Banking
{
    public partial interface IAccountListModelFactory
    {
        Task<AccountListSearchModel> PrepareAccountListSearchModelAsync(AccountListSearchModel searchModel);
        Task<AccountListListModel> PrepareAccountListListModelAsync(AccountListSearchModel searchModel, string parentId);
    }
    public partial class AccountListModelFactory : IAccountListModelFactory
    {
        private readonly IBankingTransactionService _bankingTransactionService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public AccountListModelFactory(
            IBankingTransactionService bankingTransactionService,
            ITraderConnectionService traderConnectionService,
            ISoftoneQueryFactory softoneQueryFactory,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _bankingTransactionService = bankingTransactionService;
            _traderConnectionService = traderConnectionService;
            _softoneQueryFactory = softoneQueryFactory; 
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<AccountListSearchModel> PrepareAccountListSearchModelAsync(AccountListSearchModel searchModel)
        {
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.AccountListModel.ListForm.Title");
            searchModel.DataKey = "bankBIC";

            return searchModel;
        }

        private async Task<IPagedList<AccountListModel>> GetPagedListAsync(AccountListSearchModel searchModel, string bankBIC)
        {
            var accessToken = await _bankingTransactionService.AuthenticateAsync();

            var accountList = new List<AccountListModel>();

            var list = await _bankingTransactionService.GetAccountListAsync(accessToken, bankBIC, "client_user_id");
            foreach (var item in list.Value.Payload)
            {
                var model = new AccountListModel();

                model.ResourceId = item.ResourceId;
                model.BankBIC = item.BankBIC;
                model.AccountTypeCode = item.AccountType.Code;
                model.AccountTypeValue = item.AccountType.Value;
                model.OverdraftLimit = item.OverdraftLimit;
                model.SerialNo = item.SerialNo;
                model.Account = item.Account;
                model.Iban = item.Iban;
                model.Currency = item.Currency;
                model.Alias = item.Alias;
                model.Product = item.Product;
                model.LedgerBalance = item.LedgerBalance;
                model.AvailableBalance = item.AvailableBalance;

                accountList.Add(model);
            }

            var query = accountList.AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.BankBIC.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<AccountListListModel> PrepareAccountListListModelAsync(AccountListSearchModel searchModel, string parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var accountLists = await GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new AccountListListModel().PrepareToGrid(searchModel, accountLists);

            return model;
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var justifyContent = new Dictionary<string, string> { ["justify-content"] = "center" };
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AccountListModel>(1, nameof(AccountListModel.ResourceId), hidden: true),
                ColumnConfig.Create<AccountListModel>(1, nameof(AccountListModel.BankBIC)),
                ColumnConfig.Create<AccountListModel>(1, nameof(AccountListModel.AccountTypeCode), hidden: true),
                ColumnConfig.Create<AccountListModel>(1, nameof(AccountListModel.AccountTypeValue)),
                ColumnConfig.Create<AccountListModel>(2, nameof(AccountListModel.OverdraftLimit), ColumnType.Decimal, hidden: true, style: textAlign),
                ColumnConfig.Create<AccountListModel>(1, nameof(AccountListModel.SerialNo), hidden: true),
                ColumnConfig.Create<AccountListModel>(1, nameof(AccountListModel.Account), hidden: true),
                ColumnConfig.Create<AccountListModel>(1, nameof(AccountListModel.Iban)),
                ColumnConfig.Create<AccountListModel>(1, nameof(AccountListModel.Currency)),
                ColumnConfig.Create<AccountListModel>(1, nameof(AccountListModel.Alias), hidden: true),
                ColumnConfig.Create<AccountListModel>(1, nameof(AccountListModel.Product), hidden: true),
                ColumnConfig.Create<AccountListModel>(2, nameof(AccountListModel.LedgerBalance), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<AccountListModel>(2, nameof(AccountListModel.AvailableBalance), ColumnType.Decimal, style : textAlign),
                ColumnConfig.CreateButton<AccountListModel>(0, ColumnType.RowButton, "account", "primary",
                    await _localizationService.GetResourceAsync("App.Common.Transactions"), textAlign, justifyContent)
            };

            return columns;
        }

    }
}