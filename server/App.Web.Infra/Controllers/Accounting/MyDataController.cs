using App.Core;
using App.Core.Domain.Accounting;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Accounting;
using App.Models.Traders;
using App.Services.Accounting;
using App.Services.ExportImport;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Controllers
{
    public partial class MyDataController : BaseProtectController
    {
        private readonly IMyDataModelFactory _myDataModelFactory;
        private readonly ITraderService _traderService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly IMyDataItemService _myDataItemService;
        private readonly ILocalizationService _localizationService;
        private readonly IExportToExcelService _exportToExcelService;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;

        public MyDataController(
            IMyDataModelFactory myDataModelFactory,
            ITraderService traderService,
            ITraderConnectionService traderConnectionService,
            IMyDataItemService myDataItemService,
            ILocalizationService localizationService,
            IExportToExcelService exportToExcelService,
            ILogger logger,
            IWorkContext workContext)
        {
            _myDataModelFactory = myDataModelFactory;
            _traderService = traderService;
            _traderConnectionService = traderConnectionService;
            _myDataItemService = myDataItemService;
            _localizationService = localizationService;
            _exportToExcelService = exportToExcelService;
            _logger = logger;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var infoModel = await _myDataModelFactory.PrepareMyDataInfoModelAsync(new MyDataInfoModel());
            var infoFormModel = await _myDataModelFactory.PrepareMyDataInfoFormModelAsync(new MyDataInfoFormModel());

            //prepare model
            var tableModel = await _myDataModelFactory.PrepareMyDataTableModelAsync(new MyDataTableModel());

            //prepare model
            var detailTableModel = await _myDataModelFactory.PrepareMyDataDetailTableModelAsync(new MyDataDetailTableModel());

            return Json(new { infoModel, infoFormModel, tableModel, detailTableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] MyDataInfoModel infoModel)
        {
            var _trader = await _traderService.GetTraderByIdAsync(infoModel.TraderId);
            if (_trader == null)
                return await AccessDenied();

            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(_trader.Id);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            //var trader = _trader.ToModel<TraderModel>();

            var responce = _myDataModelFactory.PrepareMyDataRequest(infoModel.IsIssuer, infoModel.StartDate, infoModel.EndDate, connectionResult.MydataUserName.Trim(), connectionResult.MydataPaswword.Trim());

            if (responce.IsSuccessful)
            {
                try
                {
                    var message = _myDataModelFactory.CheckIfMyDataTooMuch(responce.Content) ? "Ο όγκος των δεδομένων\r\nυπερβαίνει το επιτρεπτό όριο και η λήψη τους γίνετε τμηματικά.\r\nΕπιλέξτε μικρότερο ημερ/κό διάστημα." : null ;

                    var list = await _myDataModelFactory.PrepareMyDataModelListAsync(infoModel.IsIssuer, infoModel.StartDate, infoModel.EndDate, responce.Content, connectionResult.CompanyId, connectionResult.Connection, connectionResult.Vat);

                    //prepare model
                    var dialogFormModel = await _myDataModelFactory.PrepareMyDataDialogFormModelAsync(new MyDataDialogFormModel(), infoModel.IsIssuer, connectionResult.CompanyId, connectionResult.Connection);

                    //prepare model
                    var detailDialogFormModel = await _myDataModelFactory.PrepareMyDataDetailDialogFormModelAsync(new MyDataDetailDialogFormModel(), connectionResult.CompanyId, connectionResult.Connection);

                    return Json(new { list, dialogFormModel, detailDialogFormModel, message });
                }
                catch
                {
                    return BadRequest(await _localizationService.GetResourceAsync("App.Errors.InternalServerError"));
                }
            }
            else 
            {
                if (responce.Status == 401 || responce.Status == 403)
                    return BadRequest(await _localizationService.GetResourceAsync("App.Errors.WrongCredentials"));
                else
                    return BadRequest(await _localizationService.GetResourceAsync("App.Errors.WrongConnection"));
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Save([FromBody] IList<MyDataItem> models, int traderId)
        {
            var trader = await _traderService.GetTraderByIdAsync(traderId);
            if (trader == null)
                return await AccessDenied();

            var traderModel = trader.ToTraderModel();
            var date = DateTime.UtcNow.Date;
            var myDataItems = await _myDataItemService.GetAllMyDataItemAsync(traderModel.Vat);
            var deletedEntities = new List<MyDataItem>();

            models = models.Where(x =>
                !string.IsNullOrEmpty(x.CounterpartVat) &&
                !string.IsNullOrEmpty(x.ProductCode) &&
                x.SeriesId > 0 &&
                x.PaymentMethodId > 0 &&
                (x.VatCategoryId > 0 || x.TaxCategoryId > 0)
                ).ToList();

            foreach (var item in models)
            {
                item.LastDateOnUtc = date;
                item.TraderVat = traderModel.Vat;

                foreach (var myDataItem in myDataItems)
                {
                    if (_myDataItemService.Equals(myDataItem, item))
                        deletedEntities.Add(myDataItem);
                }
            }

            await _myDataItemService.DeleteMyDataItemAsync(deletedEntities);
            await _myDataItemService.InsertMyDataItemAsync(models);

            return Ok();
        }

        public virtual async Task<IActionResult> ExportToExcel([FromBody] IList<MyDataExport> list)
        {
            try
            {
                var bytes = await _exportToExcelService.MyDataExportToXlsxAsync(list);
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