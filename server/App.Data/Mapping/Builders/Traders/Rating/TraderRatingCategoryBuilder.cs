using App.Core.Domain.Traders;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderRatingCategoryBuilder : NopEntityBuilder<TraderRatingCategory>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TraderRatingCategory.Description)).AsString(100).NotNullable();
        }
    }
}
