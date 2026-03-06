using App.Core.Domain.Traders;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderGroupBuilder : NopEntityBuilder<TraderGroup>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TraderGroup.Description)).AsString(100).NotNullable();
        }
    }
}
