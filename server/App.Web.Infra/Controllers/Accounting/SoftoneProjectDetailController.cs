using App.Core.Domain.Logging;
using App.Models.Accounting;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Security;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Accounting.Controllers
{
    public partial class SoftoneProjectDetailController : BaseProtectController
    {
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ISoftoneProjectDetailModelFactory _softoneProjectDetailModelFactory;

        public SoftoneProjectDetailController(
            ITraderConnectionService traderConnectionService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            ISoftoneProjectDetailModelFactory softoneProjectDetailModelFactory)
        {
            _traderConnectionService = traderConnectionService;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _softoneProjectDetailModelFactory = softoneProjectDetailModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _softoneProjectDetailModelFactory.PrepareSoftoneProjectDetailSearchModelAsync(new SoftoneProjectDetailSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] SoftoneProjectDetailSearchModel searchModel, int parentId)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(parentId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            //prepare model
            var model = await _softoneProjectDetailModelFactory.PrepareSoftoneProjectDetailListModelAsync(searchModel, connectionResult);

            await _customerActivityService.InsertActivityOnceAsync(ActivityLogTypeType.SoftoneProject);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetProjectName(int traderId, int projectId)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var projectName = await _softoneProjectDetailModelFactory.PrepareSoftoneProjectDetailNameAsync(projectId, connectionResult.Connection, connectionResult.CompanyId);

            return Json(new { projectName });
        }

    }
}