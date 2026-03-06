using App.Core;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Models.Extensions;
using App.Models.Banking;
using App.Services.Banking;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Banking
{
    public partial interface IUserConnectionBankModelFactory
    {
        Task<UserConnectionBankSearchModel> PrepareUserConnectionBankSearchModelAsync(UserConnectionBankSearchModel searchModel);
        Task<UserConnectionBankListModel> PrepareUserConnectionBankListModelAsync(UserConnectionBankSearchModel searchModel, int parentId);
        Task<UserConnectionBankConfigModel> PrepareUserConnectionBankConfigModelAsync(UserConnectionBankConfigModel configModel, int traderId);
    }
    public partial class UserConnectionBankModelFactory : IUserConnectionBankModelFactory
    {
        private readonly IBankingTransactionService _bankingTransactionService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public UserConnectionBankModelFactory(
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

        public virtual async Task<UserConnectionBankSearchModel> PrepareUserConnectionBankSearchModelAsync(UserConnectionBankSearchModel searchModel)
        {
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.UserConnectionBankModel.ListForm.Title");
            searchModel.DataKey = "connectionId";

            return searchModel;
        }

        private async Task<IPagedList<UserConnectionBankModel>> GetPagedListAsync(UserConnectionBankSearchModel searchModel, int traderId)
        {
            var accessToken = await _bankingTransactionService.AuthenticateAsync();

            var banks = await _bankingTransactionService.UserConnectionsAsync(accessToken, "client_user_id");

            var query = banks.Value.Payload.Banks.Select(x => 
            {
                var model = new UserConnectionBankModel();

                model.ConnectionId = x.ConnectionId;
                model.Country = x.Country;
                model.BankBIC = x.BankBIC;
                model.DisplayName = x.DisplayName;
                model.Status = x.Status;
                model.ValidUntil = x.ValidUntil;

                return model;
            }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.BankBIC.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<UserConnectionBankListModel> PrepareUserConnectionBankListModelAsync(UserConnectionBankSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var userConnectionBanks = await GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new UserConnectionBankListModel().PrepareToGrid(searchModel, userConnectionBanks);

            return model;
        }

        private Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<UserConnectionBankModel>(1, nameof(UserConnectionBankModel.BankBIC)),
                ColumnConfig.Create<UserConnectionBankModel>(2, nameof(UserConnectionBankModel.DisplayName)),
                ColumnConfig.Create<UserConnectionBankModel>(2, nameof(UserConnectionBankModel.Status)),
                ColumnConfig.Create<UserConnectionBankModel>(2, nameof(UserConnectionBankModel.ValidUntil), ColumnType.Date)
            };

            return Task.FromResult(columns);
        }

        public virtual Task<UserConnectionBankConfigModel> PrepareUserConnectionBankConfigModelAsync(UserConnectionBankConfigModel configModel, int traderId)
        {
            var right1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<UserConnectionBankConfigModel>(nameof(UserConnectionBankConfigModel.DateFrom), FieldType.Date)
            };

            var right2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<UserConnectionBankConfigModel>(nameof(UserConnectionBankConfigModel.DateTo), FieldType.Date)
            };

            var fields = FieldConfig.CreateFields(new string[]
            { "col-12 md:col-3", "col-12 md:col-3" }, right1, right2);

            configModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            var date = DateTime.UtcNow;
            configModel.DateFrom = new DateTime(date.Year, date.Month, 1).Date; ;
            configModel.DateTo = configModel.DateFrom.AddMonths(1).AddDays(-1).Date;

            return Task.FromResult(configModel);
        }

    }
}