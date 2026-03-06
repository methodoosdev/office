using App.Automation.Hubs;
using App.Automation.Pages.Accounting;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class ListingF4RetrieveController : BaseController
    {
        private IHubContext<ProgressHub> _hub;

        public ListingF4RetrieveController(IHubContext<ProgressHub> hub)
        {
            _hub = hub;
        }

        [HttpPost]
        public async Task<IActionResult> List(string traderName, string vat, int year, int month, string userName, string password, string connectionId)
        {
            var result = new DtoListResponse<ListingF4Result>();

            try
            {
                await using (var page = new ListingF4RetrievePage(connectionId))
                {
                    var title = "Αποδεικτικό υποβολής Φ4";
                    //if (!string.IsNullOrEmpty(connectionId))
                    //    await _hub.Clients.Client(connectionId).SendAsync("progressLabel", $"{traderName.ToSubstring()}");

                    try
                    {
                        if (await page.Login(userName, password))
                        {
                            //await page.SendProgressLabelAsync($"{traderName.ToSubstring()}");
                            var list = await page.Execute(vat, year, month);
                            result.AddRange(list);
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
