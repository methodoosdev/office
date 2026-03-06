using App.Core;
using App.Core.Domain.Logging;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Accounting;
using App.Models.Accounting;
using App.Services.Common;
using App.Services.Customers;
using App.Services.ExportImport;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Web.Accounting.Controllers
{
    public partial class ESendController : BaseProtectController
    {
        private readonly IESendFactory _eSendFactory;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ILocalizationService _localizationService;
        private readonly IExportToExcelService _exportToExcelService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly PlaywrightHttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;

        public ESendController(
            IESendFactory eSendFactory,
            ITraderConnectionService traderConnectionService,
            ILocalizationService localizationService,
            IExportToExcelService exportToExcelService,
            ICustomerActivityService customerActivityService,
            PlaywrightHttpClient httpClient,
            ILogger logger,
            IWorkContext workContext)
        {
            _eSendFactory = eSendFactory;
            _traderConnectionService = traderConnectionService;
            _localizationService = localizationService;
            _exportToExcelService = exportToExcelService;
            _customerActivityService = customerActivityService;
            _httpClient = httpClient;
            _logger = logger;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _eSendFactory.PrepareESendSearchModelAsync(new ESendSearchModel());

            //prepare model
            var tableModel = await _eSendFactory.PrepareESendTableModelAsync(new ESendTableModel());

            return Json(new { searchModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ESendSearchModel searchModel, string connectionId)
        {
            TraderConnectionResult connectionResult = searchModel.NotSoftOne
                ? await _traderConnectionService.GetTraderAsync(searchModel.TraderId)
                : await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);

            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            //try
            //{
            //    var _fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
            //    var _importFromExcelService = EngineContext.Current.Resolve<IImportFromExcelService>();
            //    var fileName = _fileProvider.GetAbsolutePath(@"C:\Users\panos\Temp\eSend.xlsx");

            //    var exclel = _importFromExcelService.ImportExcel<ESendItem>(fileName, "ESendModel", 3);

            //}
            //catch (Exception exc)
            //{
            //}

            IList<ESendModel> list = new List<ESendModel>();

            var custActivity = new CustomerActivityResult();

            //var trader = connection.Trader;
            //var traderName = trader.FullName();
            //var companyId = connection.Trader.CompanyId;
            //var logistikiProgramTypeId = connection.Trader.LogistikiProgramTypeId;

            var eSendFromBody = new ESendFromBody
            {
                TraderName = connectionResult.TraderName,
                UserName = connectionResult.TaxisUserName.Trim(),
                Password = connectionResult.TaxisPassword.Trim(),
                Date = searchModel.Period.Date,
                ConnectionId = connectionId
            };

            var result = await _httpClient.SendAsync(HttpMethod.Post, "api/esend/list", eSendFromBody);
            if (result.Success)
            {
                var response = JsonConvert.DeserializeObject<DtoListResponse<ESendDto>>(result.Content);
                if (response.Success)
                {
                    if (response.List.Count == 0)
                        custActivity.AddSuccess($"<b>Δεν υπάρχουν εγγραφές:</b> {connectionResult.TraderName}");
                    else
                    {
                        // update traderKad with new items
                        list = searchModel.NotSoftOne
                            ? await _eSendFactory.PrepareNotESendModelListAsync(response.List, connectionResult.LogistikiProgramTypeId)
                            : await _eSendFactory.PrepareESendModelListAsync(response.List, connectionResult.CompanyId, connectionResult.Connection, connectionResult.LogistikiProgramTypeId);

                        custActivity.AddSuccess(response.Message);
                    }
                }
                else
                    custActivity.AddError(response.Error);
            }
            else
                custActivity.AddError(result.Error);

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.ESend, custActivity.ToString());

            return Json(list);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportToExcel([FromBody] IList<ESendModel> list)
        {
            try
            {
                var bytes = await _exportToExcelService.ESendModelToXlsxAsync(list);
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