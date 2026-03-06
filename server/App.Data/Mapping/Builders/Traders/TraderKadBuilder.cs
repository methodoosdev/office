using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderKadBuilder : NopEntityBuilder<TraderKad>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TraderKad.Code)).AsString(100).NotNullable()
                .WithColumn(nameof(TraderKad.TraderId)).AsInt32().ForeignKey<Trader>();
        }
    }
}
