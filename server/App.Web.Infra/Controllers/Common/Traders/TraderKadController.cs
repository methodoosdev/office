using App.Core;
using App.Core.Domain.Logging;
using App.Core.Domain.Traders;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Trader;
using App.Core.Infrastructure.Mapper;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Offices;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.Traders;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    [CheckCustomerPermission(true)]
    public partial class TraderKadController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ITraderKadService _traderKadService;
        private readonly ILocalizationService _localizationService;
        private readonly ITraderKadModelFactory _traderKadModelFactory;
        private readonly IBusinessRegistryModelFactory _businessRegistryModelFactory;
        private readonly IAccountingOfficeService _accountingOfficeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly PlaywrightHttpClient _httpClient;

        public TraderKadController(
            ITraderService traderService,
            ITraderKadService traderKadService,
            ILocalizationService localizationService,
            ITraderKadModelFactory traderKadModelFactory,
            IBusinessRegistryModelFactory businessRegistryModelFactory,
            IAccountingOfficeService accountingOfficeService,
            ICustomerActivityService customerActivityService,
            PlaywrightHttpClient httpClient)
        {
            _traderService = traderService;
            _traderKadService = traderKadService;
            _localizationService = localizationService;
            _traderKadModelFactory = traderKadModelFactory;
            _businessRegistryModelFactory = businessRegistryModelFactory;
            _accountingOfficeService = accountingOfficeService;
            _customerActivityService = customerActivityService;
            _httpClient = httpClient;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderKadModelFactory.PrepareTraderKadSearchModelAsync(new TraderKadSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderKadSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _traderKadModelFactory.PrepareTraderKadListModelAsync(searchModel, parentId);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _traderKadService.DeleteTraderKadAsync((await _traderKadService.GetTraderKadsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderKads.Errors.TryToDelete");
            }
        }

        private async Task UpdateTraderKadsByTraderAsync(IList<TraderKad> traderKads, int traderId)
        {
            //get previous records
            var allTraderKads = await _traderKadService.GetAllTraderKadsAsync(traderId);
            var traderKadsToDelete = allTraderKads.Where(x => x.GroupId == 0).ToList();

            //delete old records
            await _traderKadService.DeleteTraderKadAsync(traderKadsToDelete);

            //insert new records
            await _traderKadService.InsertTraderKadAsync(traderKads);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Import([FromBody] ICollection<int> selectedIds, string connectionId)
        {
            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();

            var traders = (await _traderService.GetTradersByIdsAsync(selectedIds.ToArray())).ToList();

            var custActivity = new CustomerActivityResult();

            foreach (var _trader in traders)
            {
                var trader = _trader.ToModel<TraderModel>();
                var traderName = trader.FullName();

                //check trader if professionType is active
                if (!_businessRegistryModelFactory.CheckIfProfessionTypeActive(office.AadeRegistryUsername.Trim(), office.AadeRegistryPassword.Trim(), trader.Vat.Trim()))
                {
                    var errorMessage = await _localizationService.GetResourceAsync("App.Errors.InvalidProfession");
                    custActivity.AddError($"<b>{traderName}:</b> {errorMessage}");

                    continue;
                }

                var format = "{0}?traderId={1}&traderName={2}&userName={3}&password={4}&connectionId={5}";
                var url = string.Format(format,
                    "api/traderKad/list",
                    trader.Id,
                    WebUtility.UrlEncode(traderName),
                    WebUtility.UrlEncode(trader.TaxisUserName.Trim()),
                    WebUtility.UrlEncode(trader.TaxisPassword.Trim()),
                    connectionId == "undefined" ? null : connectionId);

                var result = await _httpClient.SendAsync(HttpMethod.Post, url);
                if (result.Success)
                {
                    var response = JsonConvert.DeserializeObject<DtoListResponse<TraderKadDto>>(result.Content);
                    if (response.Success)
                    {
                        if (response.List.Count == 0)
                            custActivity.AddSuccess($"<b>Δεν υπάρχουν ΚΑΔ:</b> {traderName}");
                        else
                        {
                            // update traderKad with new items
                            var traderKads = response.List.Select(x => AutoMapperConfiguration.Mapper.Map<TraderKad>(x)).ToList();
                            await UpdateTraderKadsByTraderAsync(traderKads, trader.Id);

                            custActivity.AddSuccess($"<b>Επιτυχής άντληση ΚΑΔ:</b> {traderName}");
                        }
                    }
                    else
                        custActivity.AddError(response.Error);
                }
                else
                    custActivity.AddError(result.Error);
            }

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.TraderKad, custActivity.ToString());

            return Ok();
        }
    }
}