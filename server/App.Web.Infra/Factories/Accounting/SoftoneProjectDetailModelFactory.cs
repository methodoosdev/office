using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Models.Extensions;
using App.Models.Accounting;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Accounting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface ISoftoneProjectDetailModelFactory
    {
        Task<string> PrepareSoftoneProjectDetailNameAsync(int projectId, string connection, int companyId);
        Task<SoftoneProjectDetailSearchModel> PrepareSoftoneProjectDetailSearchModelAsync(SoftoneProjectDetailSearchModel searchModel);
        Task<SoftoneProjectDetailListModel> PrepareSoftoneProjectDetailListModelAsync(SoftoneProjectDetailSearchModel searchModel, TraderConnectionResult connectionResult);
    }
    public partial class SoftoneProjectDetailModelFactory : ISoftoneProjectDetailModelFactory
    {
        private readonly IAppDataProvider _dataProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;

        public SoftoneProjectDetailModelFactory(
            IAppDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService)
        {
            _dataProvider = dataProvider;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
        }

        public virtual async Task<string> PrepareSoftoneProjectDetailNameAsync(int projectId, string connection, int companyId)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var pProjectId = new LinqToDB.Data.DataParameter("pProjectId", projectId);

            var items = await _dataProvider.QueryAsync<SoftoneProjectItemQueryResult>(connection, SoftoneProjectDetailQuery.Project, pCompanyId, pProjectId);

            var item = items.FirstOrDefault();
            
            return $"{item?.Code} - {item?.Description}";
        }

        public virtual async Task<SoftoneProjectDetailSearchModel> PrepareSoftoneProjectDetailSearchModelAsync(SoftoneProjectDetailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.SoftoneProjectDetailModel.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<SoftoneProjectDetailListModel> PrepareSoftoneProjectDetailListModelAsync(SoftoneProjectDetailSearchModel searchModel, TraderConnectionResult connectionResult)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pProjectId = new LinqToDB.Data.DataParameter("pProjectId", searchModel.ProjectId);

            var incomes = new int[] { 1353, 1351, 1361, 1553 };
            var expenses = new int[] { 1251, 1653, 1253, 1453, 1261 };
            var collections = new int[] { 1381, 1413, 1415, 1581, 1414, 1313 }; //, 1414, 1313 };
            var payments = new int[] { 1281, 1412, 1416, 1681, 1481, 1212, 1312 }; //, 1212, 1312 };

            var results = await _dataProvider.QueryAsync<SoftoneProjectDetailQueryResult>(connectionResult.Connection, SoftoneProjectDetailQuery.All, pCompanyId, pProjectId);

            var query = results.SelectAwait(async x =>
            {
                //var amount = x.InvoiceTypeId == 3103 ? x.Amount * -1 : x.Amount;
                //var fpaAmount = x.InvoiceTypeId == 3103 ? x.FpaAmount * -1 : x.FpaAmount;

                var model = new SoftoneProjectDetailModel();
                model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(x.CreatedOnUtc, DateTimeKind.Utc);
                model.Invoice = x.Invoice ?? "";
                model.InvoiceType = x.InvoiceType ?? "";
                model.TraderId = x.TraderId ?? "";
                model.TraderName = x.TraderName ?? "";
                model.Fpa = x.VatAmount;
                model.Comments = x.Comments;

                if (incomes.Contains(x.SoSource))
                {
                    model.Income = x.Amount;
                    model.IncomeFpa = x.FpaAmount;
                }
                else if (expenses.Contains(x.SoSource))
                {
                    model.Expenses = x.Amount;
                    model.ExpensesFpa = x.FpaAmount;
                }
                else if (collections.Contains(x.SoSource))
                {
                    if (x.SodType == 12)
                        model.Payment = x.FpaAmount;
                    else
                        model.Collection = x.FpaAmount;
                }
                else if (payments.Contains(x.SoSource))
                {
                    if (x.SodType == 13)
                        model.Collection = x.FpaAmount;
                    else
                        model.Payment = x.FpaAmount;
                }

                return model;
            });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Invoice.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.InvoiceType.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.TraderId.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.TraderName.ContainsIgnoreCase(searchModel.QuickSearch)
                );
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            var all = await query.ToListAsync();

            var totals = new SoftoneProjectDetailModel
            {
                Id = all.Count(),
                Income = all.Sum(x => x.Income),
                IncomeFpa = all.Sum(x => x.IncomeFpa),
                Expenses = all.Sum(x => x.Expenses),
                ExpensesFpa = all.Sum(x => x.ExpensesFpa),
                Collection = all.Sum(x => x.Collection),
                Payment = all.Sum(x => x.Payment),
                Fpa = all.Sum(x => x.Fpa)
            };

            var softoneProjects = await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);

            //prepare grid model
            var model = new SoftoneProjectDetailListModel().PrepareToGrid(searchModel, softoneProjects);

            model.Draw = JsonConvert.SerializeObject(totals, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            //model.Data = model.Data.Concat(new[] { totals }).ToList();

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<SoftoneProjectDetailModel>(0, nameof(SoftoneProjectDetailModel.CreatedOn), ColumnType.Date, style: centerAlign),
                ColumnConfig.Create<SoftoneProjectDetailModel>(1, nameof(SoftoneProjectDetailModel.Invoice)),
                ColumnConfig.Create<SoftoneProjectDetailModel>(2, nameof(SoftoneProjectDetailModel.InvoiceType)),
                ColumnConfig.Create<SoftoneProjectDetailModel>(4, nameof(SoftoneProjectDetailModel.Comments), hidden: true),
                ColumnConfig.Create<SoftoneProjectDetailModel>(3, nameof(SoftoneProjectDetailModel.TraderId), hidden : true),
                ColumnConfig.Create<SoftoneProjectDetailModel>(4, nameof(SoftoneProjectDetailModel.TraderName), hidden : true),
                ColumnConfig.Create<SoftoneProjectDetailModel>(5, nameof(SoftoneProjectDetailModel.Income), ColumnType.Decimal, width: 120, style: rightAlign),
                ColumnConfig.Create<SoftoneProjectDetailModel>(6, nameof(SoftoneProjectDetailModel.IncomeFpa), ColumnType.Decimal, width: 120, style: rightAlign),
                ColumnConfig.Create<SoftoneProjectDetailModel>(7, nameof(SoftoneProjectDetailModel.Expenses), ColumnType.Decimal, width: 120, style: rightAlign),
                ColumnConfig.Create<SoftoneProjectDetailModel>(8, nameof(SoftoneProjectDetailModel.ExpensesFpa), ColumnType.Decimal, width: 120, style: rightAlign),
                ColumnConfig.Create<SoftoneProjectDetailModel>(9, nameof(SoftoneProjectDetailModel.Collection), ColumnType.Decimal, width: 120, style: rightAlign),
                ColumnConfig.Create<SoftoneProjectDetailModel>(10, nameof(SoftoneProjectDetailModel.Payment), ColumnType.Decimal, width: 120, style: rightAlign),
                ColumnConfig.Create<SoftoneProjectDetailModel>(11, nameof(SoftoneProjectDetailModel.Fpa), ColumnType.Decimal, width: 120, style: rightAlign, hidden: true)
            };

            return columns;
        }
    }
}