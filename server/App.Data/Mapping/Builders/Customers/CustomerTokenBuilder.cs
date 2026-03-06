using FluentMigrator.Builders.Create.Table;
using App.Core.Domain.Customers;
using App.Data.Extensions;

namespace App.Data.Mapping.Builders.Customers
{
    public partial class CustomerTokenBuilder : NopEntityBuilder<CustomerToken>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerToken.RefreshTokenIdHash)).AsString(450).NotNullable()
                .WithColumn(nameof(CustomerToken.RefreshTokenIdHashSource)).AsString(450).Nullable()
                .WithColumn(nameof(CustomerToken.RefreshTokenExpiresDateTime)).AsDateTimeOffset().NotNullable()
                .WithColumn(nameof(CustomerToken.AccessTokenExpiresDateTime)).AsDateTimeOffset().NotNullable()
                .WithColumn(nameof(CustomerToken.CustomerId)).AsInt32().ForeignKey<Customer>();
        }
    }
}