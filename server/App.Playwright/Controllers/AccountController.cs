using App.Automation.Pages;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Playwright.Controllers
{
    public class AccountController : BaseController
    {
        public IActionResult List()
        {
            return Ok("Server runing!");
        }

        [HttpPost]
        public async Task<IActionResult> Alive()
        {
            var alive = false;

            try
            {
                await using (var page = new PingPlaywrightPage())
                {
                    alive = true;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                alive = false;
            }

            return new JsonResult(alive);
        }
    }
}
