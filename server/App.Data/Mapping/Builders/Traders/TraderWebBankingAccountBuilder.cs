using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    //public partial class TraderWebBankingAccountBuilder : NopEntityBuilder<TraderWebBankingAccount>
    //{
    //    public override void MapEntity(CreateTableExpressionBuilder table)
    //    {
    //        table
    //            .WithColumn(nameof(TraderWebBankingAccount.Description)).AsString(100).NotNullable()
    //            .WithColumn(nameof(TraderWebBankingAccount.UserName)).AsString(100).NotNullable()
    //            .WithColumn(nameof(TraderWebBankingAccount.Password)).AsString(100).NotNullable()
    //            .WithColumn(nameof(TraderWebBankingAccount.TraderId)).AsInt32().ForeignKey<Trader>();
    //    }
    //}
}
