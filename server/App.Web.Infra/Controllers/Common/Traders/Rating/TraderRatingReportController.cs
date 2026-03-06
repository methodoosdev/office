using App.Services.Common;
using App.Services.Localization;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.Traders;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    [CheckCustomerPermission(true)]
    public partial class TraderRatingReportController : BaseProtectController
    {
        private readonly ITraderRatingReportModelFactory _traderRatingReportModelFactory;
        private readonly ISqlConnectionService _connectionService;
        private readonly ILocalizationService _localizationService;

        public TraderRatingReportController(ITraderRatingReportModelFactory traderRatingReportModelFactory,
            ISqlConnectionService connectionService,
            ILocalizationService localizationService)
        {
            _traderRatingReportModelFactory = traderRatingReportModelFactory;
            _connectionService = connectionService;
            _localizationService = localizationService;
        }

        [HttpPost]
        public virtual async Task<IActionResult> ByEmployee()
        {
            //prepare model
            var columns = _traderRatingReportModelFactory.GetByEmployeeColumnsConfig();

            //prepare model
            var data = _traderRatingReportModelFactory.GetByEmployeeList();

            var title = await _localizationService.GetResourceAsync("App.Models.TraderRatingByEmployeeModel.Title");

            return Json(new { columns, data, title });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ByDepartment()
        {
            //prepare model
            var columns = _traderRatingReportModelFactory.GetByDepartmentColumnsConfig();

            //prepare model
            var data = _traderRatingReportModelFactory.GetByDepartmentList();

            var title = await _localizationService.GetResourceAsync("App.Models.TraderRatingByDepartmentModel.Title");

            return Json(new { columns, data, title });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ByTrader()
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.Office);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            //prepare model
            var columns = _traderRatingReportModelFactory.GetByTraderColumnsConfig();

            //prepare model
            var data = await _traderRatingReportModelFactory.GetByTraderListAsync(result.Connection);

            var title = await _localizationService.GetResourceAsync("App.Models.TraderRatingByTraderModel.Title");

            return Json(new { columns, data, title });
        }

        [HttpPost]
        public virtual async Task<IActionResult> BySummaryTable()
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.Office);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            //prepare model
            var columns = _traderRatingReportModelFactory.SummaryTableModelColumnsConfig();

            //prepare model
            var data = await _traderRatingReportModelFactory.SummaryTableModelListAsync(result.Connection);

            var title = await _localizationService.GetResourceAsync("App.Models.SummaryTableModel.Title");

            return Json(new { columns, data, title });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ByValuationTable()
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.Office);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            //prepare model
            var columns = _traderRatingReportModelFactory.ValuationTableModelColumnsConfig();

            //prepare model
            var data = await _traderRatingReportModelFactory.ValuationTableModelListAsync(result.Connection);

            var title = await _localizationService.GetResourceAsync("App.Models.ValuationTableResult.Title");

            return Json(new { columns, data, title });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ByValuationTrader()
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.Office);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            //prepare model
            var columns = _traderRatingReportModelFactory.ValuationTraderResultColumnsConfig();

            //prepare model
            var data = await _traderRatingReportModelFactory.ValuationTraderResultListAsync(result.Connection);

            var title = await _localizationService.GetResourceAsync("App.Models.ValuationTraderResult.Title");

            return Json(new { columns, data, title });
        }
    }
}