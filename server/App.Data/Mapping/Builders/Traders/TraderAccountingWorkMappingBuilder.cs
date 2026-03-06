using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderAccountingWorkMappingBuilder : NopEntityBuilder<TraderAccountingWorkMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TraderAccountingWorkMapping), nameof(TraderAccountingWorkMapping.TraderId)))
                    .AsInt32().ForeignKey<Trader>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TraderAccountingWorkMapping), nameof(TraderAccountingWorkMapping.AccountingWorkId)))
                    .AsInt32().ForeignKey<AccountingWork>().PrimaryKey();
        }
    }
}
