using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using App.Core;
using App.Data;
using App.Services.Customers;

namespace App.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute confirming that user with "Employee" customer role has appropriate vendor account associated (and active)
    /// </summary>
    public sealed class ValidateEmployeeAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public ValidateEmployeeAttribute(bool ignore = false) : base(typeof(ValidateEmployeeFilter))
        {
            IgnoreFilter = ignore;
            Arguments = new object[] { ignore };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter { get; }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter confirming that user with "Employee" customer role has appropriate vendor account associated (and active)
        /// </summary>
        private class ValidateEmployeeFilter : IAsyncAuthorizationFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly ICustomerService _customerService;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public ValidateEmployeeFilter(bool ignoreFilter, IWorkContext workContext, ICustomerService customerService)
            {
                _ignoreFilter = ignoreFilter;
                _customerService = customerService;
                _workContext = workContext;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private async Task ValidateEmployeeAsync(AuthorizationFilterContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //check whether this filter has been overridden for the Action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter)
                    .OfType<ValidateEmployeeAttribute>()
                    .FirstOrDefault();

                //ignore filter (the action is available even if the current customer isn't a vendor)
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                //whether current customer is employee
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsEmployeeAsync(customer))
                    return;

                //ensure that this user has active employee record associated
                var employee = await _workContext.GetCurrentEmployeeAsync();
                if (employee == null)
                    context.Result = new ChallengeResult();
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                await ValidateEmployeeAsync(context);
            }

            #endregion
        }

        #endregion
    }
}