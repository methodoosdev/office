using App.Core.Domain.Customers;
using App.Models.Customers;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Customers
{
    /// <summary>
    /// Represents the customer role model factory
    /// </summary>
    public partial interface ICustomerRoleModelFactory
    {
        /// <summary>
        /// Prepare customer role search model
        /// </summary>
        /// <param name="searchModel">Customer role search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role search model
        /// </returns>
        Task<CustomerRoleSearchModel> PrepareCustomerRoleSearchModelAsync(CustomerRoleSearchModel searchModel);

        /// <summary>
        /// Prepare paged customer role list model
        /// </summary>
        /// <param name="searchModel">Customer role search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role list model
        /// </returns>
        Task<CustomerRoleListModel> PrepareCustomerRoleListModelAsync(CustomerRoleSearchModel searchModel);

        /// <summary>
        /// Prepare customer role model
        /// </summary>
        /// <param name="model">Customer role model</param>
        /// <param name="customerRole">Customer role</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role model
        /// </returns>
        Task<CustomerRoleModel> PrepareCustomerRoleModelAsync(CustomerRoleModel model, CustomerRole customerRole, bool excludeProperties = false);
        Task<CustomerRoleFormModel> PrepareCustomerRoleFormModelAsync(CustomerRoleFormModel formModel);
    }
}