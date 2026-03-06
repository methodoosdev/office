using App.Core;
using App.Core.Domain.Customers;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
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
    public partial class CustomerRoleController : BaseProtectController
    {
        #region Fields

        private readonly ICustomerRoleModelFactory _customerRoleModelFactory;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CustomerRoleController(
            ICustomerRoleModelFactory customerRoleModelFactory,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _customerRoleModelFactory = customerRoleModelFactory;
            _customerService = customerService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _customerRoleModelFactory.PrepareCustomerRoleSearchModelAsync(new CustomerRoleSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] CustomerRoleSearchModel searchModel)
        {
            //prepare model
            var model = await _customerRoleModelFactory.PrepareCustomerRoleListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _customerRoleModelFactory.PrepareCustomerRoleModelAsync(new CustomerRoleModel(), null);

            //prepare form
            var formModel = await _customerRoleModelFactory.PrepareCustomerRoleFormModelAsync(new CustomerRoleFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] CustomerRoleModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var customerRole = model.ToEntity<CustomerRole>();
                await _customerService.InsertCustomerRoleAsync(customerRole);

                return Json(customerRole.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get a customer role with the specified id
            var customerRole = await _customerService.GetCustomerRoleByIdAsync(id);
            if (customerRole == null)
                return await AccessDenied();

            //prepare model
            var model = await _customerRoleModelFactory.PrepareCustomerRoleModelAsync(null, customerRole);

            //prepare form
            var formModel = await _customerRoleModelFactory.PrepareCustomerRoleFormModelAsync(new CustomerRoleFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IActionResult> Edit([FromBody] CustomerRoleModel model, bool continueEditing)
        {
            //try to get a customer role with the specified id
            var customerRole = await _customerService.GetCustomerRoleByIdAsync(model.Id);
            if (customerRole == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    if (customerRole.IsSystemRole && !model.Active)
                        throw new NopException(await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Fields.Active.CantEditSystem"));

                    if (customerRole.IsSystemRole && !customerRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new NopException(await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Fields.SystemName.CantEditSystem"));

                    customerRole = model.ToEntity(customerRole);
                    await _customerService.UpdateCustomerRoleAsync(customerRole);

                    return Json(customerRole.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.CustomerRoles.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var customerRole = await _customerService.GetCustomerRoleByIdAsync(id);
            if (customerRole == null)
                return await AccessDenied();

            try
            {
                await _customerService.DeleteCustomerRoleAsync(customerRole);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.CustomerRoles.Errors.TryToDelete");
            }
        }

        #endregion
    }
}