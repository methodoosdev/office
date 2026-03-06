using App.Core;
using App.Core.Domain.Customers;
using App.Core.Infrastructure;
using App.Models.Customers;
using App.Services.Customers;
using App.Services.Security;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Customers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Customers
{
    public partial class CustomerSecurityController : BaseProtectController
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerPermissionService _customerPermissionService;
        private readonly ICustomerSecurityModelFactory _customerSecurityModelFactory;

        public CustomerSecurityController(
            ICustomerService customerService,
            ICustomerPermissionService customerPermissionService,
            ICustomerSecurityModelFactory customerSecurityModelFactory)
        {
            _customerService = customerService;
            _customerPermissionService = customerPermissionService;
            _customerSecurityModelFactory = customerSecurityModelFactory;
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _customerSecurityModelFactory.PrepareCustomerPermissionMappingModelAsync(new CustomerPermissionMappingModel());

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] Dictionary<string, IList<string>> form)
        {
            var permissions = await _customerPermissionService.GetAllCustomerPermissionsAsync();
            var customers = await _customerService.GetRegisterCustomersAsync();

            foreach (var cr in customers)
            {
                var formKey = "allow_" + cr.Id;

                var customerPermissionSystemNamesToRestrict =
                    form.TryGetValue(formKey, out IList<string> list) ? list : new List<string>();

                foreach (var pr in permissions)
                {
                    var allow = customerPermissionSystemNamesToRestrict.Contains(pr.SystemName.ToCamelCase());

                    if (allow == await _customerPermissionService.AuthorizeAsync(pr.SystemName, cr.Id))
                        continue;

                    if (allow)
                    {
                        await _customerPermissionService.InsertCustomerPermissionCustomerMappingAsync(new CustomerPermissionCustomerMapping { CustomerPermissionId = pr.Id, CustomerId = cr.Id });
                    }
                    else
                    {
                        await _customerPermissionService.DeleteCustomerPermissionCustomerMappingAsync(pr.Id, cr.Id);
                    }

                    await _customerPermissionService.UpdateCustomerPermissionAsync(pr);
                }
            }

            return Ok();
        }
    }
}