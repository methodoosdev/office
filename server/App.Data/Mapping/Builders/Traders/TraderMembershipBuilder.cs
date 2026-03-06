using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderMembershipBuilder : NopEntityBuilder<TraderMembership>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TraderMembership.TraderId)).AsInt32().ForeignKey<Trader>();
        }
    }
}
