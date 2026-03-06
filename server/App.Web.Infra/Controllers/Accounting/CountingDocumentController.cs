using App.Core;
using App.Core.Domain.Logging;
using App.Core.Domain.Traders;
using App.Models.Accounting;
using App.Services.Common;
using App.Services.Common.Pdf;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Accounting.Controllers
{
    public partial class CountingDocumentController : BaseProtectController
    {
        private readonly ICountingDocumentFactory _countingDocumentFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public CountingDocumentController(
            ICountingDocumentFactory countingDocumentFactory,
            ILocalizationService localizationService,
            IHtmlToPdfService htmlToPdfService,
            ITraderConnectionService traderConnectionService,
            ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _countingDocumentFactory = countingDocumentFactory;
            _localizationService = localizationService;
            _htmlToPdfService = htmlToPdfService;
            _traderConnectionService = traderConnectionService;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _countingDocumentFactory.PrepareCountingDocumentSearchModelAsync(new CountingDocumentSearchModel());

            //prepare model
            var tableModel = await _countingDocumentFactory.PrepareCountingDocumentTableModelAsync(new CountingDocumentTableModel());

            return Json(new { tableModel, searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] CountingDocumentSearchModel searchModel)
        {            
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var date = await _dateTimeHelper.ConvertToUserTimeAsync(searchModel.Periodos);

            var data = await _countingDocumentFactory.PrepareCountingDocumentListAsync(connectionResult, date.Year);

            //await _customerActivityService.InsertActivityOnceAsync(ActivityLogTypeType.CountingDocument);

            return Json(new { data });
        }
    }
}