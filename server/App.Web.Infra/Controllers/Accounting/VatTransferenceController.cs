using App.Core.Domain.Traders;
using App.Models.Accounting;
using App.Models.Payroll;
using App.Services.Common;
using App.Services.Localization;
using App.Services.Security;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Common.Models.Payroll;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Accounting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Accounting
{
    public partial class VatTransferenceController : BaseProtectController
    {
        private readonly ISqlConnectionService _connectionService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ILocalizationService _localizationService;
        private readonly IVatTransferenceFactory _vatTransferenceFactory;
        private readonly IPermissionService _permissionService;

        public VatTransferenceController(
            ISqlConnectionService connectionService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ITraderConnectionService traderConnectionService,
            IVatTransferenceFactory vatTransferenceFactory)
        {
            _connectionService = connectionService;
            _traderConnectionService = traderConnectionService;
            _permissionService = permissionService;
            _vatTransferenceFactory = vatTransferenceFactory;
            _localizationService = localizationService;
        }
        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _vatTransferenceFactory.PrepareVatTransferenceSearchModelAsync(new VatTransferenceSearchModel());

            //prepare model
            var tableModel = await _vatTransferenceFactory.PrepareVatTransferenceTableModelAsync(new VatTransferenceTableModel());

            return Json(new { searchModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] VatTransferenceSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var model = await _vatTransferenceFactory.PrepareVatTransferenceListAsync(connectionResult, searchModel);

            return Json(model);
        }
    }
}
