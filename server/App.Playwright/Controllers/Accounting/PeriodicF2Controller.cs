using App.Automation.Pages.Accounting;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class PeriodicF2Controller : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Retrieve(string traderName, string vat, int pageKindTypeId, int year, int from, int to, string userName, string password, string connectionId)
        {
            var result = new DtoListResponse<PeriodicF2Result>();

            try
            {
                await using (var page = new PeriodicF2RetrievePage(connectionId))
                {
                    var title = "Περιοδική ΦΠΑ - Άντληση";
                    //if (!string.IsNullOrEmpty(connectionId))
                    //    await _hub.Clients.Client(connectionId).SendAsync("progressLabel", $"{model.TraderName.ToSubstring()}");

                    try
                    {
                        if (await page.Login(userName, password))
                        {
                            //await page.SendProgressLabelAsync($"{model.TraderName.ToSubstring()}");
                            var list = await page.Execute(vat, pageKindTypeId, year, from, to);
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

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] PeriodicF2Result model, string traderName, string vat, int pageKindTypeId, bool f007, int year, int from, int to, string userName, string password, string connectionId)
        {
            var result = new DtoListResponse<string>();

            try
            {
                await using (var page = new PeriodicF2SubmitPage(connectionId))
                {
                    var title = "Περιοδική ΦΠΑ - Δημιουργία";
                    //if (!string.IsNullOrEmpty(connectionId))
                    //    await _hub.Clients.Client(connectionId).SendAsync("progressLabel", $"{model.TraderName.ToSubstring()}");

                    try
                    {
                        if (await page.Login(userName, password))
                        {
                            //await page.SendProgressLabelAsync($"{model.TraderName.ToSubstring()}");
                            var registrationNumber = await page.Execute(model, vat, pageKindTypeId, f007, year, from, to);
                            result.AddItem(registrationNumber);
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


        [HttpPost]
        public async Task<IActionResult> IdentityPayment(string traderName, string vat, int pageKindTypeId, bool f007, int year, int from, int to, string userName, string password, string connectionId)
        {
            var result = new DtoListResponse<PeriodicF2Result>();

            try
            {
                await using (var page = new PeriodicF2IdentityPage(connectionId))
                {
                    var title = "Περιοδική ΦΠΑ - Ταυτότητα";
                    //if (!string.IsNullOrEmpty(connectionId))
                    //    await _hub.Clients.Client(connectionId).SendAsync("progressLabel", $"{model.TraderName.ToSubstring()}");

                    try
                    {
                        if (await page.Login(userName, password))
                        {
                            //await page.SendProgressLabelAsync($"{model.TraderName.ToSubstring()}");
                            await page.Execute(vat, pageKindTypeId, f007, year, from, to);
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
