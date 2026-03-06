using App.Automation.Pages.Financial;
using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Financial;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class EfkaRetrieveController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> List(int index, int traderId, string traderName, string userName, string password, string vat, string amka, string connectionId)
        {
            var result = new DtoListResponse<FinancialObligationDto>();

            try
            {
                await using (var page = new EfkaRetrievePage(connectionId))
                {
                    var title = "Εισφορές Μη μισθωτών";
                    try
                    {
                        await page.Login(userName, password, vat, amka);
                        if (page.LoginIn)
                        {
                            await page.SendProgressLabelAsync($"{index}.{title}-{traderName.ToSubstring()}");
                            var retrieves = await page.Execute(traderId);
                            result.AddRange(retrieves);
                            result.AddMessage($"<b>{title}:</b> {traderName}");
                        }
                        else
                        {
                            result.AddError($"<b>Σύνδεση:</b> {traderName}. Σφάλμα: Οι κωδικοί Taxis ή το ΑΦΜ ή το ΑΜΚΑ είναι λανθασμένα.");
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
