namespace App.Core.Domain.Customers
{
    public partial class CustomerPermissionCustomerMapping : BaseEntity
    {
        public int CustomerPermissionId { get; set; }
        public int CustomerId { get; set; }
    }
}