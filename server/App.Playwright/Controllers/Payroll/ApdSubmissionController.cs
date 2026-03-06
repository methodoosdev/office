using App.Automation.Hubs;
using App.Automation.Pages.Payroll;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Payroll;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class ApdSubmissionController : BaseController
    {
        private IHubContext<ProgressHub> _hub;

        public ApdSubmissionController(IHubContext<ProgressHub> hub)
        {
            _hub = hub;
        }

        [HttpPost]
        public async Task<IActionResult> List(int index, string traderName, int month, int year, string userName, string password, string amIka, string connectionId)
        {
            var result = new DtoListResponse<ApdSubmissionDto>();

            try
            {
                await using (var page = new ApdSubmissionPage(connectionId))
                {
                    var title = "Υποβολή ΑΠΔ";
                    //if (!string.IsNullOrEmpty(connectionId))
                    //    await _hub.Clients.Client(connectionId).SendAsync("progressLabel", $"{index}.{traderName.ToSubstring()}");

                    try
                    {
                        await page.Login(userName, password, amIka);
                        if (page.LoginIn)
                        {
                            //await page.SendProgressLabelAsync($"{index}.{traderName.ToSubstring()}");
                            var item = await page.Execute(month, year);
                            result.AddRange(item);
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

            return Json(result);
        }
    }
}
