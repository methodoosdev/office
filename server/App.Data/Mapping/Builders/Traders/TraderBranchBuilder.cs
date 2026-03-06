using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderBranchBuilder : NopEntityBuilder<TraderBranch>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TraderBranch.TraderId)).AsInt32().ForeignKey<Trader>();
        }
    }
}
