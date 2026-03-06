using App.Models.Banking;
using App.Services.Localization;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Banking;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Banking
{
    public partial class UserConnectionBankController : BaseProtectController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IUserConnectionBankModelFactory _userConnectionBankModelFactory;

        public UserConnectionBankController(
            ILocalizationService localizationService,
            IUserConnectionBankModelFactory userConnectionBankModelFactory)
        {
            _localizationService = localizationService;
            _userConnectionBankModelFactory = userConnectionBankModelFactory;
        }

        public virtual async Task<IActionResult> Config(int parentId)
        {
            //prepare model
            var configModel = await _userConnectionBankModelFactory.PrepareUserConnectionBankConfigModelAsync(new UserConnectionBankConfigModel(), parentId);

            return Json(new { configModel });
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _userConnectionBankModelFactory.PrepareUserConnectionBankSearchModelAsync(new UserConnectionBankSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] UserConnectionBankSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _userConnectionBankModelFactory.PrepareUserConnectionBankListModelAsync(searchModel, parentId);

            return Json(model);
        }
    }
}