using App.Core.Domain.Customers;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Customers;
using App.Services;
using App.Services.Customers;
using App.Services.Localization;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Customers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Customers
{
    public partial class CustomerPermissionController : BaseProtectController
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerPermissionService _customerPermissionService;
        private readonly ICustomerPermissionModelFactory _customerPermissionModelFactory;
        private readonly ICustomerPermissionCustomerMappingService _customerPermissionCustomerMappingService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ILocalizationService _localizationService;

        public CustomerPermissionController(
            ICustomerService customerService,
            ICustomerPermissionService customerPermissionService,
            ICustomerPermissionModelFactory customerPermissionModelFactory,
            ICustomerPermissionCustomerMappingService customerPermissionCustomerMappingService,
            IModelFactoryService modelFactoryService,
            ILocalizationService localizationService)
        {
            _customerService = customerService;
            _customerPermissionService = customerPermissionService;
            _customerPermissionModelFactory = customerPermissionModelFactory;
            _customerPermissionCustomerMappingService = customerPermissionCustomerMappingService;
            _modelFactoryService = modelFactoryService;
            _localizationService = localizationService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _customerPermissionModelFactory.PrepareCustomerPermissionSearchModelAsync(new CustomerPermissionSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] CustomerPermissionSearchModel searchModel)
        {
            //prepare model
            var model = await _customerPermissionModelFactory.PrepareCustomerPermissionListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _customerPermissionModelFactory.PrepareCustomerPermissionModelAsync(new CustomerPermissionModel(), null);

            //prepare form
            var formModel = await _customerPermissionModelFactory.PrepareCustomerPermissionFormModelAsync(new CustomerPermissionFormModel(), model);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] CustomerPermissionModel model)
        {
            if (ModelState.IsValid)
            {
                var customerPermission = model.ToEntity<CustomerPermission>();
                await _customerPermissionService.InsertCustomerPermissionAsync(customerPermission);

                return Json(customerPermission.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var customerPermission = await _customerPermissionService.GetCustomerPermissionByIdAsync(id);
            if (customerPermission == null)
                return await AccessDenied();

            //prepare model
            var model = await _customerPermissionModelFactory.PrepareCustomerPermissionModelAsync(null, customerPermission);

            //prepare form
            var formModel = await _customerPermissionModelFactory.PrepareCustomerPermissionFormModelAsync(new CustomerPermissionFormModel(), model);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] CustomerPermissionModel model)
        {
            //try to get entity with the specified id
            var customerPermission = await _customerPermissionService.GetCustomerPermissionByIdAsync(model.Id);
            if (customerPermission == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    customerPermission = model.ToEntity(customerPermission);
                    await _customerPermissionService.UpdateCustomerPermissionAsync(customerPermission);

                    return Json(customerPermission.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.CustomerPermissions.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var customerPermission = await _customerPermissionService.GetCustomerPermissionByIdAsync(id);
            if (customerPermission == null)
                return await AccessDenied();

            try
            {
                await _customerPermissionService.DeleteCustomerPermissionAsync(customerPermission);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.CustomerPermissions.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _customerPermissionService.DeleteCustomerPermissionAsync((await _customerPermissionService.GetCustomerPermissionsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.CustomerPermissions.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> AutoInsertMissingPermission()
        {
            //prepare available
            var availableCustomerPermission = _customerPermissionModelFactory.GetCustomerPermissionModelList();

            var newCustomerPermissions = new List<CustomerPermission>();
            var repoCustomerPermissions = await _customerPermissionService.GetAllCustomerPermissionsAsync();
            foreach (var available in availableCustomerPermission)
            {
                if (!repoCustomerPermissions.Any(x => x.SystemName == available.SystemName))
                    if (!newCustomerPermissions.Any(x => x.SystemName == available.SystemName))
                        newCustomerPermissions.Add(available.ToEntity<CustomerPermission>());
            }

            if (newCustomerPermissions.Count > 0)
                await _customerPermissionService.InsertCustomerPermissionAsync(newCustomerPermissions);
            else
                return await BadRequestMessageAsync("App.Errors.NoRecordsForInsertion");

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportMapping([FromBody] CustomerPermissionsByCustomersModel model)
        {
            foreach (var parentId in model.Customers.ToArray())
            {
                var customer = await _customerService.GetCustomerByIdAsync(parentId);
                if (customer == null)
                    continue;

                var customerPermissions = await _customerPermissionService.GetCustomerPermissionsByIdsAsync(model.CustomerPermissions.ToArray());

                foreach (var selected in customerPermissions)
                {
                    await _customerPermissionCustomerMappingService.InsertCustomerPermissionCustomerAsync(selected, customer);
                }

            }

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> RemoveMapping([FromBody] CustomerPermissionsByCustomersModel model)
        {
            foreach (var parentId in model.Customers.ToArray())
            {
                var customer = await _customerService.GetCustomerByIdAsync(parentId);
                if (customer == null)
                    continue;

                var customerPermissions = await _customerPermissionService.GetCustomerPermissionsByIdsAsync(model.CustomerPermissions.ToArray());

                foreach (var selected in customerPermissions)
                {
                    await _customerPermissionCustomerMappingService.RemoveCustomerPermissionCustomerAsync(selected, customer);
                }

            }

            return Ok();
        }

    }
}