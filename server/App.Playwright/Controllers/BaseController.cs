using App.Core.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace App.Playwright.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    public class BaseController : Controller 
    {
        protected string GetLanguage() 
        {
            // Access the custom header value
            string headerValue = Request.Headers["X-Custom-Header"];

            // Optionally, check if the header exists
            if (string.IsNullOrEmpty(headerValue))
            {
                return "el-GR";
            }

            return headerValue;
        }

        public override JsonResult Json(object data)
        {
            var serializerSettings = EngineContext.Current.Resolve<IOptions<MvcNewtonsoftJsonOptions>>()?.Value?.SerializerSettings
                ?? new JsonSerializerSettings();

            serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;

            return base.Json(data, serializerSettings);
        }
    }
}
