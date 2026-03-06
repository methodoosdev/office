using App.Models.Banking;
using App.Services.Localization;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Banking;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Banking
{
    public partial class AvailableBankController : BaseProtectController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IAvailableBankModelFactory _availableBankModelFactory;

        public AvailableBankController(
            ILocalizationService localizationService,
            IAvailableBankModelFactory availableBankModelFactory)
        {
            _localizationService = localizationService;
            _availableBankModelFactory = availableBankModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _availableBankModelFactory.PrepareAvailableBankSearchModelAsync(new AvailableBankSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AvailableBankSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _availableBankModelFactory.PrepareAvailableBankListModelAsync(searchModel, parentId);

            return Json(model);
        }
    }
}