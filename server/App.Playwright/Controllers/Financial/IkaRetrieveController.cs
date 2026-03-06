using App.Automation.Pages.Financial;
using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Financial;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class IkaRetrieveController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> List(int index, int traderId, string traderName, string userName, string password, string connectionId)
        {
            var result = new DtoListResponse<FinancialObligationDto>();

            try
            {
                await using (var page = new IkaRetrievePage(connectionId))
                {
                    var title = "ΕΦΚΑ Εργοδοτικές εισφόρες";
                    var subTitle = "ΕΦΚΑ Εργοδ.Εισφ.";
                    try
                    {
                        if (await page.Login(userName, password))
                        {
                            await page.SendProgressLabelAsync($"{index}.{subTitle}-{traderName.ToSubstring()}");
                            var retrieves = await page.Execute(traderId, title);
                            result.AddRange(retrieves);
                            result.AddMessage($"<b>{title}:</b> {traderName}");
                        }
                        else
                        {
                            result.AddError($"<b>Σύνδεση:</b> {traderName}. Σφάλμα: Μη έγκυροι κωδικοί ΙΚΑ Εργοδότη");
                        }
                    }
                    catch (Exception exception)
                    {
                        result.AddError($"<b>ΚΕΑΟ-{title}:</b> {traderName}. Σφάλμα: {exception.Message}");
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
