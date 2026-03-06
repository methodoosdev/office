using App.Core.Domain.Customers;
using App.Models.Customers;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Security;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Customers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Customers
{
    public partial class CustomerOnlineController : BaseProtectController
    {
        private readonly ICustomerOnlineService _customerOnlineService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerOnlineModelFactory _customerOnlineModelFactory;

        public CustomerOnlineController(
            ICustomerOnlineService customerOnlineService,
            ILocalizationService localizationService,
            ICustomerOnlineModelFactory customerOnlineModelFactory)
        {
            _customerOnlineService = customerOnlineService;
            _localizationService = localizationService;
            _customerOnlineModelFactory = customerOnlineModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _customerOnlineModelFactory.PrepareCustomerOnlineSearchModelAsync(new CustomerOnlineSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] CustomerOnlineSearchModel searchModel)
        {
            //prepare model
            var model = await _customerOnlineModelFactory.PrepareCustomerOnlineListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var customerOnline = await _customerOnlineService.GetCustomerOnlineByIdAsync(id);
            if (customerOnline == null)
                return await AccessDenied();

            try
            {
                await _customerOnlineService.DeleteCustomerOnlineAsync(customerOnline);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.CustomerOnlines.Errors.TryToDelete");
            }
        }
    }
}