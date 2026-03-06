using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Models;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services;
using App.Services.Localization;
using App.Web.Infra.Queries.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ISrfTraderModelFactory
    {
        Task<SrfTraderSearchModel> PrepareSrfTraderSearchModelAsync(SrfTraderSearchModel searchModel);
        Task<SrfTraderListModel> PrepareSrfTraderListModelAsync(SrfTraderSearchModel searchModel, string connection);
    }
    public partial class SrfTraderModelFactory : ISrfTraderModelFactory
    {
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public SrfTraderModelFactory(
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<SrfTraderModel>> GetPagedListAsync(SrfTraderSearchModel searchModel, string connection)
        {
            var logistikiProgramTypes = await LogistikiProgramType.SoftOne.ToSelectionItemListAsync();
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();

            var srfTradersList = await _dataProvider.QueryAsync<SrfTraderModel>(connection, SrfTradersQuery.Get);

            var query = (from srf in srfTradersList.AsEnumerable()
                         from lpt in logistikiProgramTypes.Where(x => x.Value == srf.LogistikiProgramTypeId).DefaultIfEmpty()
                         from cbt in categoryBookTypes.Where(x => x.Value == srf.CategoryBookTypeId).DefaultIfEmpty()
                         select new SrfTraderModel
                         {
                             SrfId = srf.SrfId,
                             LastName = srf.LastName,
                             FirstName = srf.FirstName,
                             Vat = srf.Vat,
                             Email = srf.Email,
                             CategoryBookTypeId = srf.CategoryBookTypeId,
                             CategoryBookTypeName = cbt?.Label ?? "",
                             TaxSystemId = srf.TaxSystemId,
                             HyperPayrollId = srf.HyperPayrollId,
                             LogistikiProgramTypeId = srf.LogistikiProgramTypeId,
                             LogistikiProgramTypeName = lpt?.Label ?? "",
                             CompanyId = srf.CompanyId,
                             EmployerIkaUserName = CommonHelper.SrfDecrypt(srf.EmployerIkaUserName),
                             EmployerIkaPassword = CommonHelper.SrfDecrypt(srf.EmployerIkaPassword),
                             TaxisUserName = CommonHelper.SrfDecrypt(srf.TaxisUserName),
                             TaxisPassword = CommonHelper.SrfDecrypt(srf.TaxisPassword),
                             OaeeUserName = CommonHelper.SrfDecrypt(srf.OaeeUserName),
                             OaeePassword = CommonHelper.SrfDecrypt(srf.OaeePassword),
                             SepeUserName = CommonHelper.SrfDecrypt(srf.SepeUserName),
                             SepePassword = CommonHelper.SrfDecrypt(srf.SepePassword),
                             SpecialTaxisUserName = CommonHelper.SrfDecrypt(srf.SpecialTaxisUserName),
                             SpecialTaxisPassword = CommonHelper.SrfDecrypt(srf.SpecialTaxisPassword),
                             EfkaUserName = CommonHelper.SrfDecrypt(srf.EfkaUserName),
                             EfkaPassword = CommonHelper.SrfDecrypt(srf.EfkaPassword),
                             LogistikiDataBaseName = CommonHelper.SrfDecrypt(srf.LogistikiDataBaseName),
                             LogistikiUsername = CommonHelper.SrfDecrypt(srf.LogistikiUsername),
                             LogistikiPassword = CommonHelper.SrfDecrypt(srf.LogistikiPassword),
                             LogistikiIpAddress = CommonHelper.SrfDecrypt(srf.LogistikiIpAddress),
                             LogistikiPort = CommonHelper.SrfDecrypt(srf.LogistikiPort)
                         }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.LastName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.FirstName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Vat.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Email.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.CategoryBookTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.LogistikiProgramTypeName.ContainsIgnoreCase(searchModel.QuickSearch)
                );
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<SrfTraderSearchModel> PrepareSrfTraderSearchModelAsync(SrfTraderSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.SrfTraderModel.ListForm.Title");
            searchModel.DataKey = "srfId";
            searchModel.Height = 520;

            return searchModel;
        }

        public virtual async Task<SrfTraderListModel> PrepareSrfTraderListModelAsync(SrfTraderSearchModel searchModel, string connection)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var srfTraders = await GetPagedListAsync(searchModel, connection);

            //prepare grid model
            var model = new SrfTraderListModel().PrepareToGrid(searchModel, srfTraders);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<SrfTraderModel>(1, nameof(SrfTraderModel.LastName)),
                ColumnConfig.Create<SrfTraderModel>(2, nameof(SrfTraderModel.FirstName)),
                ColumnConfig.Create<SrfTraderModel>(3, nameof(SrfTraderModel.Vat)),
                ColumnConfig.Create<SrfTraderModel>(4, nameof(SrfTraderModel.Email)),
                ColumnConfig.Create<SrfTraderModel>(5, nameof(SrfTraderModel.CategoryBookTypeName)),
                ColumnConfig.Create<SrfTraderModel>(6, nameof(SrfTraderModel.HyperPayrollId), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(7, nameof(SrfTraderModel.EmployerIkaUserName), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(8, nameof(SrfTraderModel.EmployerIkaPassword), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(9, nameof(SrfTraderModel.TaxisUserName), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(10, nameof(SrfTraderModel.TaxisPassword), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(11, nameof(SrfTraderModel.OaeeUserName), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(12, nameof(SrfTraderModel.OaeePassword), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(13, nameof(SrfTraderModel.SepePassword), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(14, nameof(SrfTraderModel.SepeUserName), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(15, nameof(SrfTraderModel.SpecialTaxisUserName), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(16, nameof(SrfTraderModel.SpecialTaxisPassword), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(17, nameof(SrfTraderModel.EfkaUserName), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(18, nameof(SrfTraderModel.EfkaPassword), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(19, nameof(SrfTraderModel.CompanyId)),
                ColumnConfig.Create<SrfTraderModel>(20, nameof(SrfTraderModel.LogistikiDataBaseName), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(21, nameof(SrfTraderModel.LogistikiUsername), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(22, nameof(SrfTraderModel.LogistikiPassword), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(23, nameof(SrfTraderModel.LogistikiIpAddress), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(24, nameof(SrfTraderModel.LogistikiPort), hidden: true),
                ColumnConfig.Create<SrfTraderModel>(25, nameof(SrfTraderModel.LogistikiProgramTypeName), hidden: true)
            };

            return columns;
        }
    }
}