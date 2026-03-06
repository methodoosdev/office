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
    /// Represents a filter attribute confirming that user with "Vendor" customer role has appropriate vendor account associated (and active)
    /// </summary>
    public sealed class ValidateTraderAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public ValidateTraderAttribute(bool ignore = false) : base(typeof(ValidateTraderFilter))
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
        /// Represents a filter confirming that user with "Trader" customer role has appropriate vendor account associated (and active)
        /// </summary>
        private class ValidateTraderFilter : IAsyncAuthorizationFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly ICustomerService _customerService;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public ValidateTraderFilter(bool ignoreFilter, IWorkContext workContext, ICustomerService customerService)
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
            private async Task ValidateTraderAsync(AuthorizationFilterContext context)
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
                if (!await _customerService.IsTraderAsync(customer))
                    return;

                //ensure that this user has active employee record associated
                var trader = await _workContext.GetCurrentTraderAsync();
                if (trader == null)
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
                await ValidateTraderAsync(context);
            }

            #endregion
        }

        #endregion
    }
}