using App.Core.Domain.Traders;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class WorkingAreaBuilder : NopEntityBuilder<WorkingArea>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WorkingArea.Description)).AsString(100).NotNullable();
        }
    }
}
