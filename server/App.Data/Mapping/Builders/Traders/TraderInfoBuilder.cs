using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderInfoBuilder : NopEntityBuilder<TraderInfo>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TraderInfo.SortDescription)).AsString(1000).NotNullable()
                .WithColumn(nameof(TraderInfo.TraderId)).AsInt32().ForeignKey<Trader>();
        }
    }
}
