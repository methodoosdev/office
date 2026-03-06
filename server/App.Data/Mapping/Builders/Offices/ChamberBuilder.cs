using App.Core.Domain.Offices;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Offices
{
    public partial class ChamberBuilder : NopEntityBuilder<Chamber>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Chamber.ChamberName)).AsString(100).NotNullable();
        }
    }
}