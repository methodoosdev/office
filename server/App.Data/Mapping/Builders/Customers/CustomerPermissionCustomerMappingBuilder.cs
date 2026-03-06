using App.Core.Domain.Customers;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Customers
{
    public partial class CustomerPermissionCustomerMappingBuilder : NopEntityBuilder<CustomerPermissionCustomerMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerPermissionCustomerMapping), nameof(CustomerPermissionCustomerMapping.CustomerPermissionId)))
                    .AsInt32().ForeignKey<CustomerPermission>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerPermissionCustomerMapping), nameof(CustomerPermissionCustomerMapping.CustomerId)))
                    .AsInt32().ForeignKey<Customer>().PrimaryKey();
        }
    }
}
