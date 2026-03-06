using App.Core.Domain.Offices;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Offices
{
    public partial class PeriodicityItemBuilder : NopEntityBuilder<PeriodicityItem>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PeriodicityItem.Paragraph)).AsString(40).NotNullable();
        }
    }
}
