using App.Automation.Pages.Financial;
using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Financial;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class DoyRetrieveController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> List(int index, int traderId, string traderName, string userName, string password, string connectionId)
        {
            var result = new DtoListResponse<FinancialObligationDto>();

            try
            {
                await using (var page = new DoyRetrievePage(connectionId))
                {
                    var title = "ΑΑΔΕ";
                    try
                    {
                        if (await page.Login(userName, password))
                        {
                            await page.SendProgressLabelAsync($"{index}.{title}-{traderName.ToSubstring()}");
                            var retrieves = await page.Execute(traderId);
                            result.AddRange(retrieves);
                            result.AddMessage($"<b>{title}:</b> {traderName}");
                        }
                        else
                        {
                            result.AddError($"<b>Σύνδεση:</b> {traderName}. Σφάλμα: Μη έγγυρα διαπιστευτήρια Taxis");
                        }
                    }
                    catch (Exception exception)
                    {
                        result.AddError($"<b>{title}:</b> {traderName}. Σφάλμα: {exception.Message}");
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
