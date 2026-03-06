using App.Core.Domain.Offices;
using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using System.Data;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderBuilder : NopEntityBuilder<Trader>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Trader.Vat)).AsString(9).NotNullable()
                .WithColumn(nameof(Trader.LastName)).AsString(150).NotNullable()
                .WithColumn(nameof(Trader.TemporarilyItems)).AsDecimal(12, 2)
                .WithColumn(nameof(Trader.ChamberId)).AsInt32().Nullable().ForeignKey<Chamber>(onDelete: Rule.None)
                .WithColumn(nameof(Trader.WorkingAreaId)).AsInt32().Nullable().ForeignKey<WorkingArea>(onDelete: Rule.None)
                .WithColumn(nameof(Trader.TraderGroupId)).AsInt32().Nullable().ForeignKey<TraderGroup>(onDelete: Rule.None);
        }
    }
}
