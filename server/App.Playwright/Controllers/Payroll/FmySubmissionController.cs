using App.Automation.Hubs;
using App.Automation.Pages.Payroll;
using App.Core.Infrastructure.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class FmySubmissionController : BaseController
    {
        private IHubContext<ProgressHub> _hub;

        public FmySubmissionController(IHubContext<ProgressHub> hub)
        {
            _hub = hub;
        }

        [HttpPost]
        public async Task<IActionResult> List(int index, string traderName, int monthFrom, int monthTo, int year, bool mySelf, string userName, string password, string connectionId)
        {
            var result = new DtoListResponse<string>();

            try
            {
                await using (var page = new FmySubmissionPage(connectionId))
                {
                    var title = "Υποβολή ΦΜΥ";
                    //if (!string.IsNullOrEmpty(connectionId))
                    //    await _hub.Clients.Client(connectionId).SendAsync("progressLabel", $"{index}.{traderName.ToSubstring()}");

                    try
                    {
                        if (await page.Login(userName, password))
                        {
                            //await page.SendProgressLabelAsync($"{index}.{traderName.ToSubstring()}");
                            var list = await page.Execute(monthFrom, monthTo, year, mySelf);
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

            return Json(result);
        }
    }
}
