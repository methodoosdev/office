using App.Core;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Customers;
using App.Services.Customers;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Customers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Customers
{
    public partial class CustomerPermissionsByCustomerController : BaseProtectController
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerPermissionService _customerPermissionService;
        private readonly ICustomerPermissionModelFactory _customerPermissionModelFactory;
        private readonly ICustomerPermissionCustomerMappingService _customerPermissionCustomerMappingService;

        public CustomerPermissionsByCustomerController(
            ICustomerService customerService,
            ICustomerPermissionService customerPermissionService,
            ICustomerPermissionModelFactory customerPermissionModelFactory,
            ICustomerPermissionCustomerMappingService customerPermissionCustomerMappingService)
        {
            _customerService = customerService;
            _customerPermissionService = customerPermissionService;
            _customerPermissionModelFactory = customerPermissionModelFactory;
            _customerPermissionCustomerMappingService = customerPermissionCustomerMappingService;
        }

        private async Task<IPagedList<CustomerPermissionModel>> GetCustomerPermissionsByCustomerIdAsync(CustomerPermissionSearchModel searchModel, int customerId)
        {
            var customerPermissions = await _customerPermissionCustomerMappingService.GetCustomerPermissionsByCustomerIdAsync(customerId);

            var query = customerPermissions.Select(x => x.ToModel<CustomerPermissionModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Name.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.SystemName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Area.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Controller.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Action.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _customerPermissionModelFactory.PrepareCustomerPermissionSearchModelAsync(new CustomerPermissionSearchModel());
            searchModel.Length = int.Parse(searchModel.AvailablePageSizes.Split(',').First());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] CustomerPermissionSearchModel searchModel, int parentId)
        {
            //try to get entity with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(parentId);
            if (customer == null)
                return await AccessDenied();

            var customers = await GetCustomerPermissionsByCustomerIdAsync(searchModel, customer.Id);

            //prepare grid model
            var model = new CustomerPermissionListModel().PrepareToGrid(searchModel, customers);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportMapping([FromBody] ICollection<int> selectedIds, int parentId)
        {
            //try to get entity with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(parentId);
            if (customer == null)
                return await AccessDenied();

            var customerPermissions = await _customerPermissionService.GetCustomerPermissionsByIdsAsync(selectedIds.ToArray());

            foreach (var selected in customerPermissions)
            {
                await _customerPermissionCustomerMappingService.InsertCustomerPermissionCustomerAsync(selected, customer);
            }

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> RemoveMapping([FromBody] ICollection<int> selectedIds, int parentId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(parentId);
            if (customer == null)
                return await AccessDenied();

            var customerPermissions = await _customerPermissionService.GetCustomerPermissionsByIdsAsync(selectedIds.ToArray());

            foreach (var selected in customerPermissions)
            {
                await _customerPermissionCustomerMappingService.RemoveCustomerPermissionCustomerAsync(selected, customer);
            }

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> InsertCustomerPermissions(int parentId, int customerId)
        {
            var parent = await _customerService.GetCustomerByIdAsync(parentId);
            var customer = await _customerService.GetCustomerByIdAsync(customerId);

            // Remove old permissions
            await _customerPermissionCustomerMappingService.RemoveCustomerPermissionsFromCustomerAsync(customer);

            // Insert permissions from parent
            await _customerPermissionCustomerMappingService.InsertCustomerPermissionsFromCustomerAsync(parent, customer);

            return Ok();
        }

    }
}