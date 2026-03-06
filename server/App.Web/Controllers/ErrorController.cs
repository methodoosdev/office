using App.Services.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public partial class ErrorController : Controller
    {
        private readonly ILocalizationService _localizationService;

        public ErrorController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }
        public async Task<IActionResult> Error()
        {
            //Response.StatusCode = StatusCodes.Status500InternalServerError;
            //return File("ErrorPage.htm", "text/html");

            return StatusCode(StatusCodes.Status500InternalServerError, await _localizationService.GetResourceAsync("App.Errors.InternalServerError"));
        }
    }
}