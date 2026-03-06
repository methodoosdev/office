using App.Automation.Hubs;
using App.Automation.Pages.Trader;
using App.Core.Infrastructure.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class TraderBoardMemberController : BaseController
    {
        private IHubContext<ProgressHub> _hub;

        public TraderBoardMemberController(IHubContext<ProgressHub> hub)
        {
            _hub = hub;
        }

        [HttpPost]
        public async Task<IActionResult> List(string traderName, string userName, string password, string connectionId)
        {
            var result = new TraderBoardMemberDto();

            try
            {
                await using (var page = new TraderBoardMemberPage(connectionId))
                {
                    var title = "’ντληση Δοικούντων από TaxisNet";
                    //if (!string.IsNullOrEmpty(connectionId))
                    //    await _hub.Clients.Client(connectionId).SendAsync("progressLabel", $"{traderName.ToSubstring()}");

                    try
                    {
                        if (await page.Login(userName, password))
                        {
                            //await page.SendProgressLabelAsync($"{traderName.ToSubstring()}");
                            var item = await page.Execute();
                            result.TraderMemberships.AddRange(item.TraderMemberships);
                            result.TraderRelationships.AddRange(item.TraderRelationships);
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
