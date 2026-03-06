using App.Core.Domain.Customers;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Customers
{
    public partial class CustomerPermissionBuilder : NopEntityBuilder<CustomerPermission>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerPermission.Name)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(CustomerPermission.SystemName)).AsString(255).NotNullable()
                .WithColumn(nameof(CustomerPermission.Area)).AsString(255).NotNullable()
                .WithColumn(nameof(CustomerPermission.Controller)).AsString(255).NotNullable()
                .WithColumn(nameof(CustomerPermission.Action)).AsString(255).NotNullable();
        }
    }
}
