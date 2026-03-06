using Microsoft.AspNetCore.Mvc;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;

namespace App.Web.Controllers
{
    //[WwwRequirement]
    //[_CheckLanguageSeoCode]
    //[_CheckAccessPublicStore]
    public abstract partial class BasePublicController : BaseController
    {
        protected virtual IActionResult InvokeHttp404()
        {
            Response.StatusCode = 404;
            return new EmptyResult();
        }
    }
}