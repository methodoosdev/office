using App.Core.Domain.Traders;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderBoardMemberTypeBuilder : NopEntityBuilder<TraderBoardMemberType>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TraderBoardMemberType.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(TraderBoardMemberType.BoardMemberTypeId)).AsInt32().NotNullable();
        }
    }
}
