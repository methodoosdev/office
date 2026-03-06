using App.Core;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Models.Extensions;
using App.Models.Banking;
using App.Services;
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
    public partial interface IAvailableBankModelFactory
    {
        Task<AvailableBankSearchModel> PrepareAvailableBankSearchModelAsync(AvailableBankSearchModel searchModel);
        Task<AvailableBankListModel> PrepareAvailableBankListModelAsync(AvailableBankSearchModel searchModel, int parentId);
    }
    public partial class AvailableBankModelFactory : IAvailableBankModelFactory
    {
        private readonly IBankingTransactionService _bankingTransactionService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public AvailableBankModelFactory(
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

        public virtual async Task<AvailableBankSearchModel> PrepareAvailableBankSearchModelAsync(AvailableBankSearchModel searchModel)
        {
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.AvailableBankModel.ListForm.Title");
            searchModel.DataKey = "bankBIC";

            return searchModel;
        }

        private async Task<IPagedList<AvailableBankModel>> GetPagedListAsync(AvailableBankSearchModel searchModel, int traderId)
        {
            var accessToken = await _bankingTransactionService.AuthenticateAsync();

            var banks = await _bankingTransactionService.GetAvailableBanksAsync(accessToken);

            var query = banks.Value.Payload.Select(x => 
            {
                var model = new AvailableBankModel();

                model.BankBIC = x.BankBIC;
                model.Country = x.Country;
                model.DisplayName = x.DisplayName;

                return model;
            }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.BankBIC.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<AvailableBankListModel> PrepareAvailableBankListModelAsync(AvailableBankSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var availableBanks = await GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new AvailableBankListModel().PrepareToGrid(searchModel, availableBanks);

            return model;
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var justifyContent = new Dictionary<string, string> { ["justify-content"] = "center" };
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AvailableBankModel>(1, nameof(AvailableBankModel.BankBIC)),
                ColumnConfig.Create<AvailableBankModel>(2, nameof(AvailableBankModel.Country)),
                ColumnConfig.Create<AvailableBankModel>(2, nameof(AvailableBankModel.DisplayName)),
                ColumnConfig.CreateButton<AvailableBankModel>(0, ColumnType.RowButton, "connection", "primary",
                    await _localizationService.GetResourceAsync("App.Common.Connection"), textAlign, justifyContent)
            };

            return columns;
        }

    }
}