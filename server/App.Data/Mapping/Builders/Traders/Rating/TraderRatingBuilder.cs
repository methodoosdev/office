using App.Core.Domain.Employees;
using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderRatingBuilder : NopEntityBuilder<TraderRating>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TraderRating.Description)).AsString(100).NotNullable()
                .WithColumn(nameof(TraderRating.TraderRatingCategoryId)).AsInt32().ForeignKey<TraderRatingCategory>()
                .WithColumn(nameof(TraderRating.DepartmentId)).AsInt32().ForeignKey<Department>();
        }
    }
}
