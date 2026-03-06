using App.Core.Domain.Traders;
using App.Core.Domain.Traders.Rating;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderRatingTraderMappingBuilder : NopEntityBuilder<TraderRatingTraderMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TraderRatingTraderMapping), nameof(TraderRatingTraderMapping.TraderId)))
                    .AsInt32().ForeignKey<Trader>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TraderRatingTraderMapping), nameof(TraderRatingTraderMapping.TraderRatingId)))
                    .AsInt32().ForeignKey<TraderRating>().PrimaryKey();
        }
    }
}
