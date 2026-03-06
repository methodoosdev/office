using App.Automation.Pages.Accounting;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class ESendController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> List([FromBody] ESendFromBody model)
        {
            var result = new DtoListResponse<ESendDto>();

            try
            {
                await using (var page = new ESendPage(model.ConnectionId))
                {
                    var title = "E-Send";
                    //if (!string.IsNullOrEmpty(connectionId))
                    //    await _hub.Clients.Client(connectionId).SendAsync("progressLabel", $"{model.TraderName.ToSubstring()}");

                    try
                    {
                        if (await page.Login(model.UserName, model.Password))
                        {
                            //await page.SendProgressLabelAsync($"{model.TraderName.ToSubstring()}");
                            var list = await page.Execute(model.Date);
                            result.AddRange(list);
                            result.AddMessage($"<b>Επιτυχής άντληση {title}:</b> {model.TraderName}");
                        }
                        else
                        {
                            result.AddError($"<b>Σύνδεση:</b> {model.TraderName}. Σφάλμα: Μη έγγυρα διαπιστευτήρια Taxis");
                        }
                    }
                    catch (Exception exception)
                    {
                        result.AddError($"<b>{title}:</b> {model.TraderName}. Σφάλμα: {exception.Message}");
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
