using App.Core.Domain.Customers;
using App.Models.Customers;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Customers
{
    public partial interface ICustomerModelFactory
    {
        Task<CustomerSearchModel> PrepareCustomerSearchModelAsync(CustomerSearchModel searchModel);
        Task<CustomerListModel> PrepareCustomerListModelAsync(CustomerSearchModel searchModel);
        Task<CustomerModel> PrepareCustomerModelAsync(CustomerModel model, Customer customer);
        Task PrepareCustomerModelHelperAsync(CustomerModelHelper model);
        Task<CustomerFormModel> PrepareCustomerFormModelAsync(CustomerFormModel formModel, CustomerModelHelper model);
        Task<CustomerDialogFormModel> PrepareCustomerDialogFormModelAsync(CustomerDialogFormModel formModel);

    }
}