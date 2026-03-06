using App.Core.Domain.Traders;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class AccountingWorkBuilder : NopEntityBuilder<AccountingWork>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AccountingWork.SortDescription)).AsString(100).NotNullable();
        }
    }
}
