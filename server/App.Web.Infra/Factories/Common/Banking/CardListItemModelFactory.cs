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
    public partial interface ICardListItemModelFactory
    {
        Task<CardListItemSearchModel> PrepareCardListItemSearchModelAsync(CardListItemSearchModel searchModel);
        Task<CardListItemListModel> PrepareCardListItemListModelAsync(CardListItemSearchModel searchModel, string parentId);
    }
    public partial class CardListItemModelFactory : ICardListItemModelFactory
    {
        private readonly IBankingTransactionService _bankingTransactionService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public CardListItemModelFactory(
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

        public virtual async Task<CardListItemSearchModel> PrepareCardListItemSearchModelAsync(CardListItemSearchModel searchModel)
        {
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.CardListItemModel.ListForm.Title");
            searchModel.DataKey = "bankBIC";

            return searchModel;
        }

        private async Task<IPagedList<CardListItemModel>> GetPagedListAsync(CardListItemSearchModel searchModel, string bankBIC)
        {
            var accessToken = await _bankingTransactionService.AuthenticateAsync();

            var cardListItem = new List<CardListItemModel>();

            var list = await _bankingTransactionService.GetCardListAsync(accessToken, bankBIC, "client_user_id");
            foreach (var item in list.Value.Cards)
            {
                var model = new CardListItemModel();

                model.Alias = item.Alias;
                model.CreditBalance = item.CreditBalance;
                model.Kind = item.Kind;
                model.CreditLine = item.CreditLine;
                model.CardHolderNameLatin = item.CardHolderNameLatin;
                model.ResourceId = item.ResourceId;
                model.BankBIC = item.BankBIC;
                model.Number = item.Number;
                model.ProductName = item.ProductName;

                cardListItem.Add(model);
            }

            var query = cardListItem.AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.BankBIC.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<CardListItemListModel> PrepareCardListItemListModelAsync(CardListItemSearchModel searchModel, string parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var cardListItems = await GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new CardListItemListModel().PrepareToGrid(searchModel, cardListItems);

            return model;
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var justifyContent = new Dictionary<string, string> { ["justify-content"] = "center" };
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<CardListItemModel>(1, nameof(CardListItemModel.Alias), hidden: true),
                ColumnConfig.Create<CardListItemModel>(1, nameof(CardListItemModel.Kind)),
                ColumnConfig.Create<CardListItemModel>(1, nameof(CardListItemModel.CardHolderNameLatin)),
                ColumnConfig.Create<CardListItemModel>(1, nameof(CardListItemModel.ResourceId), hidden: true),
                ColumnConfig.Create<CardListItemModel>(1, nameof(CardListItemModel.BankBIC)),
                ColumnConfig.Create<CardListItemModel>(1, nameof(CardListItemModel.Number)),
                ColumnConfig.Create<CardListItemModel>(1, nameof(CardListItemModel.ProductName)),
                ColumnConfig.Create<CardListItemModel>(2, nameof(CardListItemModel.CreditBalance), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<CardListItemModel>(2, nameof(CardListItemModel.CreditLine), ColumnType.Decimal, style : textAlign),
                ColumnConfig.CreateButton<CardListItemModel>(0, ColumnType.RowButton, "card", "primary",
                    await _localizationService.GetResourceAsync("App.Common.Transactions"), textAlign, justifyContent)
            };

            return columns;
        }

    }
}