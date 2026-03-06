using App.Models.Banking;
using App.Services.Localization;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Banking;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Banking
{
    public partial class AccountListController : BaseProtectController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IAccountListModelFactory _accountListModelFactory;

        public AccountListController(
            ILocalizationService localizationService,
            IAccountListModelFactory accountListModelFactory)
        {
            _localizationService = localizationService;
            _accountListModelFactory = accountListModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _accountListModelFactory.PrepareAccountListSearchModelAsync(new AccountListSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AccountListSearchModel searchModel, string parentId)
        {
            //prepare model
            var model = await _accountListModelFactory.PrepareAccountListListModelAsync(searchModel, parentId);

            return Json(model);
        }
    }
}