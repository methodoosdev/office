using App.Automation.Pages.Trader;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Trader;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class TraderKadController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> List(int traderId, string traderName, string userName, string password, string connectionId)
        {
            var result = new DtoListResponse<TraderKadDto>();

            try
            {
                await using (var page = new TraderKadPage(connectionId))
                {
                    try
                    {
                        var traderKads = await page.Execute(userName, password, traderId);
                        result.AddRange(traderKads);
                    }
                    catch (Exception exception)
                    {
                        result.AddError($"<b>Αντληση ΚΑΔ:</b> {traderName}. Σφάλμα: {exception.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError(ex.Message);
            }

            return new JsonResult(result);
        }
    }
}
