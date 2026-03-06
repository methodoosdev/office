using App.Models.Banking;
using App.Services.Localization;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Banking;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Banking
{
    public partial class CardListItemController : BaseProtectController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ICardListItemModelFactory _cardListItemModelFactory;

        public CardListItemController(
            ILocalizationService localizationService,
            ICardListItemModelFactory cardListItemModelFactory)
        {
            _localizationService = localizationService;
            _cardListItemModelFactory = cardListItemModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _cardListItemModelFactory.PrepareCardListItemSearchModelAsync(new CardListItemSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] CardListItemSearchModel searchModel, string parentId)
        {
            //prepare model
            var model = await _cardListItemModelFactory.PrepareCardListItemListModelAsync(searchModel, parentId);

            return Json(model);
        }
    }
}