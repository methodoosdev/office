using App.Automation.Pages.Trader;
using App.Core.Infrastructure.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class TraderBranchController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> List(int traderId, string traderName, string userName, string password, string connectionId)
        {
            var result = new TraderBranchPageDto();

            try
            {
                await using (var page = new TraderBranchPage(connectionId))
                {
                    try
                    {
                        var traderBranchPageResult = await page.Execute(userName, password, traderId);
                        result.TraderKads.AddRange(traderBranchPageResult.TraderKads);
                        result.TraderBranches.AddRange(traderBranchPageResult.TraderBranches);
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
