using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Banking;
using App.Models.Traders;
using App.Services.Banking;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Banking
{
    public partial interface IBankingTraderModelFactory
    {
        Task<TraderSearchModel> PrepareScriptTraderSearchModelAsync(TraderSearchModel searchModel);
        Task<TraderListModel> PrepareScriptTraderListModelAsync(TraderSearchModel searchModel);
        Task<TraderModel> PrepareTraderModelAsync(TraderModel model, Trader trader);
        Task<TraderFormModel> PrepareScriptTraderFormModelAsync(TraderFormModel formModel);
        Task<AccountTransactionSearchModel> PrepareAccountTransactionSearchModelAsync(AccountTransactionSearchModel searchModel);
        Task<AccountTransactionListModel> PrepareAccountTransactionsAsync(AccountTransactionSearchModel searchModel,
            string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo);
    }
    public partial class BankingTraderModelFactory : IBankingTraderModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly IBankingTransactionService _bankingTransactionService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public BankingTraderModelFactory(ITraderService traderService,
            ITraderConnectionService traderConnectionService,
            IBankingTransactionService bankingTransactionService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _traderConnectionService = traderConnectionService;
            _bankingTransactionService = bankingTransactionService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<TraderSearchModel> PrepareScriptTraderSearchModelAsync(TraderSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderListModel> PrepareScriptTraderListModelAsync(TraderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));


            var query = _traderService.Table.AsEnumerable().Select(x =>
            {
                var model = x.ToModel<TraderModel>();

                model.FullName = model.FullName() ?? "";
                model.Vat = model.Vat ?? "";
                model.Doy = model.Doy ?? "";
                model.Email = model.Email ?? "";

                return model;
            }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.FullName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Vat.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Doy.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Email.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.Where(c =>
                c.CategoryBookTypeId == (int)CategoryBookType.C ||
                c.CategoryBookTypeId == (int)CategoryBookType.B);

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            var traders = await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);

            //prepare grid model
            var model = new TraderListModel().PrepareToGrid(searchModel, traders);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderModel>(1, nameof(TraderModel.FullName), ColumnType.RouterLink),
                ColumnConfig.Create<TraderModel>(2, nameof(TraderModel.Vat)),
                ColumnConfig.Create<TraderModel>(2, nameof(TraderModel.Email)),
                ColumnConfig.Create<TraderModel>(2, nameof(TraderModel.ConnectionAccountingActive), ColumnType.Checkbox, filterType: "boolean"),
            };

            return columns;
        }

        public virtual Task<TraderModel> PrepareTraderModelAsync(TraderModel model, Trader trader)
        {
            if (trader != null)
            {
                //fill in model values from the entity
                model ??= trader.ToModel<TraderModel>();
            }

            return Task.FromResult(model);
        }

        public virtual async Task<TraderFormModel> PrepareScriptTraderFormModelAsync(TraderFormModel formModel)
        {
            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LastName), FieldType.Text, _readonly: true, hideLabel: true),
            };

            var right1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Vat), FieldType.Text, _readonly: true, hideLabel: true)
            };

            var right2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ConnectionAccountingActive), FieldType.Checkbox, disabled: true, hideLabel: true)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3", "col-12 md:col-3" }, left, right1, right2);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        public virtual async Task<AccountTransactionSearchModel> PrepareAccountTransactionSearchModelAsync(AccountTransactionSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.AccountTransactionModel.ListForm.Title");
            searchModel.DataKey = "amount";

            return searchModel;
        }

        public virtual async Task<AccountTransactionListModel> PrepareAccountTransactionsAsync(AccountTransactionSearchModel searchModel,
            string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo)
        {
            var accessToken = await _bankingTransactionService.AuthenticateAsync();

            var accountTransactions = new List<AccountTransactionModel>();

            var list = await _bankingTransactionService.GetAccountTransactionsAsync(
                accessToken, bankBIC, "client_user_id", resourceId, dateFrom, dateTo);

            foreach (var item in list.Value.Payload)
            {
                var model = new AccountTransactionModel();

                model.Date = item.Date;
                model.Amount = item.Amount;
                model.Currency = item.Currency;
                model.CreditDebit = item.CreditDebit;
                model.Valeur = item.Valeur;
                model.Description = item.Description;
                model.Reference = item.Reference;
                model.Timestamp = item.Timestamp;
                model.Status = item.Status;
                model.Trans = item.Trans;
                model.TransDescription = item.TransDescription;
                model.RelatedAccount = item.RelatedAccount;
                model.RelatedName = item.RelatedName;

                accountTransactions.Add(model);
            }

            var query = accountTransactions.AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            var pagedList = await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);

            //prepare grid model
            var listModel = new AccountTransactionListModel().PrepareToGrid(searchModel, pagedList);

            return listModel;
        }
    }
}