using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderParentMappingBuilder : NopEntityBuilder<TraderParentMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TraderParentMapping), nameof(TraderParentMapping.TraderId)))
                    .AsInt32().ForeignKey<Trader>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TraderParentMapping), nameof(TraderParentMapping.ParentId)))
                    .AsInt32().ForeignKey<Trader>(onDelete: System.Data.Rule.None).PrimaryKey();
        }
    }
}
