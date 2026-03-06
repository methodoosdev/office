using App.Core;
using App.Core.Domain.Logging;
using App.Core.Infrastructure;
using App.Data;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Framework.Mvc.Filters
{
    public sealed class CheckCustomerPermissionAttribute : TypeFilterAttribute
    {
        public CheckCustomerPermissionAttribute(bool ignore = false) : base(typeof(CheckCustomerPermissionFilter))
        {
            IgnoreFilter = ignore;
            Arguments = new object[] { ignore };
        }

        public bool IgnoreFilter { get; }

        private class CheckCustomerPermissionFilter : IAsyncAuthorizationFilter
        {
            private readonly bool _ignoreFilter;
            private readonly ICustomerPermissionService _customerPermissionService;
            private readonly ICustomerService _customerService;
            private readonly IWorkContext _workContext;

            public CheckCustomerPermissionFilter(bool ignoreFilter, 
                ICustomerPermissionService customerPermissionService, ICustomerService customerService, IWorkContext workContext
                )
            {
                _ignoreFilter = ignoreFilter;
                _customerPermissionService = customerPermissionService;
                _customerService = customerService;
                _workContext = workContext;
            }

            private async Task CheckCustomerPermissionAsync(AuthorizationFilterContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //check whether this filter has been overridden for the Action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter)
                    .OfType<CheckCustomerPermissionAttribute>()
                    .FirstOrDefault();

                //ignore filter (the action is available even if navigation is not allowed)
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                //whether current customer is admin
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (await _customerService.IsAdminAsync(customer))
                    return;

                //get action and controller names
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var area = context.ActionDescriptor.RouteValues["area"];
                var actionName = actionDescriptor?.ActionName;
                var controllerName = actionDescriptor?.ControllerName;

                if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                    return;

                var permission = $"{area}:{controllerName}:{actionName}";
                //check whether current customer has access to a public store
                if (await _customerPermissionService.AuthorizeAsync(permission))
                    return;

                var customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();
                await customerActivityService.InsertActivityAsync(ActivityLogTypeType.Customer, permission);

                //customer hasn't access to a public store
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                var errorMessage = await localizationService.GetResourceAsync("App.Errors.AccessDenied");
                context.Result = new UnauthorizedObjectResult(errorMessage+"01");
            }

            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                await CheckCustomerPermissionAsync(context);
            }
        }
    }
}