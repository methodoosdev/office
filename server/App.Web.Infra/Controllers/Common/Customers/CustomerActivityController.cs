using App.Core;
using App.Models.Customers;
using App.Services.Logging;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.Customers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Customers
{
    public partial class CustomerActivityController : BaseProtectController
    {
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerActivityModelFactory _customerActivityModelFactory;
        private readonly IWorkContext _workContext;

        public CustomerActivityController(
            ICustomerActivityService customerActivityService,
            ICustomerActivityModelFactory customerActivityModelFactory,
            IWorkContext workContext)
        {
            _customerActivityService = customerActivityService;
            _customerActivityModelFactory = customerActivityModelFactory;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _customerActivityModelFactory.PrepareActivityLogCustomerSearchModelAsync(new CustomerActivityLogSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] CustomerActivityLogSearchModel searchModel)
        {
            //prepare model
            var model = await _customerActivityModelFactory.PrepareCustomerActivityLogListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var activityLog = await _customerActivityService.GetActivityByIdAsync(id);
            if (activityLog == null)
                return await AccessDenied();

            try
            {
                await _customerActivityService.DeleteActivityAsync(activityLog);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                {
                    var activities = await _customerActivityService.GetActivitiesByIdsAsync(selectedIds.ToArray());
                    await _customerActivityService.DeleteActivityAsync(activities.ToList());
                }

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Errors.TryToDelete");
            }
        }

        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> LastActivity()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return await AccessDenied();

            var comment = await _customerActivityModelFactory.PrepareLastActivityLogAsync(customer.Id);

            return Json(new { comment });
        }
    }
}