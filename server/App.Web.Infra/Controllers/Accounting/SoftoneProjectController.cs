using App.Models.Accounting;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Accounting.Controllers
{
    public partial class SoftoneProjectController : BaseProtectController
    {
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ILocalizationService _localizationService;
        private readonly ISoftoneProjectModelFactory _softoneProjectModelFactory;

        public SoftoneProjectController(
            ITraderConnectionService traderConnectionService,
            ILocalizationService localizationService,
            ISoftoneProjectModelFactory softoneProjectModelFactory)
        {
            _traderConnectionService = traderConnectionService;
            _localizationService = localizationService;
            _softoneProjectModelFactory = softoneProjectModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _softoneProjectModelFactory.PrepareSoftoneProjectSearchModelAsync(new SoftoneProjectSearchModel());

            //prepare model
            var infoModel = await _softoneProjectModelFactory.PrepareSoftoneProjectInfoModelAsync(new SoftoneProjectInfoModel());
            var infoFormModel = await _softoneProjectModelFactory.PrepareSoftoneProjectInfoFormModelAsync(new SoftoneProjectInfoFormModel());

            return Json(new { searchModel, infoModel, infoFormModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] SoftoneProjectSearchModel searchModel, int parentId)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(parentId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            //prepare model
            var model = await _softoneProjectModelFactory.PrepareSoftoneProjectListModelAsync(searchModel, connectionResult);

            return Json(model);
        }
    }
}