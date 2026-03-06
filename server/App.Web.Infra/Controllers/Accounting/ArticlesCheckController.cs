using App.Core;
using App.Models.Accounting;
using App.Models.Traders;
using App.Services.Common;
using App.Services.ExportImport;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Accounting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Accounting
{
    public partial class ArticlesCheckController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly IExportToExcelService _exportToExcelService;
        private readonly ISqlConnectionService _connectionService;
        private readonly IArticlesCheckModelFactory _articlesCheckModelFactory;

        public ArticlesCheckController(
            ITraderService traderService,
            ITraderConnectionService traderConnectionService,
            IExportToExcelService exportToExcelService,
            ISqlConnectionService connectionService,
            IArticlesCheckModelFactory articlesCheckModelFactory)
        {
            _traderService = traderService;
            _traderConnectionService = traderConnectionService;
            _exportToExcelService = exportToExcelService;
            _connectionService = connectionService;
            _articlesCheckModelFactory = articlesCheckModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            //searchModel
            var searchModel = await _articlesCheckModelFactory.PrepareArticlesCheckSearchModelAsync(new ArticlesCheckSearchModel());

            //formModel
            var formModel = await _articlesCheckModelFactory.PrepareArticlesCheckSearchFormModelAsync(new ArticlesCheckSearchFormModel(), 0, result.Connection);

            //tableModel
            var tableModel = await _articlesCheckModelFactory.PrepareArticlesCheckTableModelAsync(new ArticlesCheckTableModel());

            return Json(new { searchModel, formModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ArticlesCheckSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var model = await _articlesCheckModelFactory.PrepareArticlesCheckModelListAsync(result.Connection, searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> View(int companyId, int nglId, int year, int period)
        {
            var soft1Connection = string.Empty;
            var trader = await _traderService.GetTraderByHyperPayrollIdAsync(companyId);
            if (trader != null)
            {
                var soft1Result = await _traderConnectionService.GetTraderConnectionAsync(trader.Id);
                if (soft1Result.Success)
                    soft1Connection = soft1Result.Connection;
            }

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            //prepare form
            var data = await _articlesCheckModelFactory.PrepareArticlesCheckAcountListAsync(
                soft1Connection, result.Connection, companyId, nglId, year, period);

            //prepare form
            var tableModel = await _articlesCheckModelFactory.PrepareArticlesCheckAccountTableModelAsync(new ArticlesCheckAccountTableModel());

            return Json(new { data, tableModel, softOneConValid = !string.IsNullOrEmpty(soft1Connection), traderName = trader?.ToTraderFullName() });
        }

        [HttpPost]
        [CheckCustomerPermission(true)]
        // hack to excel - must fix it
        public virtual async Task<IActionResult> ExportToPdf([FromBody] IList<ArticlesCheckAccountModel> list)
        {
            try
            {
                var bytes = await _exportToExcelService.ArticlesCheckAccountModelToXlsxAsync(list);
                return File(bytes, MimeTypes.TextXlsx);
            }
            catch //(Exception exc)
            {
                //await _logger.ErrorAsync("Failed to ExportToExcel.", exc);
                return await BadRequestMessageAsync("App.Errors.FailedExportToExcel");
            }
        }

    }
}
