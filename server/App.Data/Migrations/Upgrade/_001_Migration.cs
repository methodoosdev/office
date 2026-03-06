using App.Core.Domain.Traders;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2023-10-01 00:00:00", "Trader new properties KEAO", MigrationProcessType.Update)]
    public class _001_Migration : Migration 
    {
        public override void Up()
        {
            // add column
            var traderTableName = nameof(Trader);

            var keaoIkaUserNameColumnName = nameof(Trader.KeaoIkaUserName);
            var keaoIkaPasswordColumnName = nameof(Trader.KeaoIkaPassword);
            var keaoOaeeUserNameColumnName = nameof(Trader.KeaoOaeeUserName);
            var keaoOaeePasswordColumnName = nameof(Trader.KeaoOaeePassword);
            var keaoEfkaUserNameColumnName = nameof(Trader.KeaoEfkaUserName);
            var keaoEfkaPasswordColumnName = nameof(Trader.KeaoEfkaPassword);

            if (!Schema.Table(traderTableName).Column(keaoIkaUserNameColumnName).Exists())
            {
                Alter.Table(traderTableName)
                    .AddColumn(keaoIkaUserNameColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(traderTableName).Column(keaoIkaPasswordColumnName).Exists())
            {
                Alter.Table(traderTableName)
                    .AddColumn(keaoIkaPasswordColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(traderTableName).Column(keaoOaeeUserNameColumnName).Exists())
            {
                Alter.Table(traderTableName)
                    .AddColumn(keaoOaeeUserNameColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(traderTableName).Column(keaoOaeePasswordColumnName).Exists())
            {
                Alter.Table(traderTableName)
                    .AddColumn(keaoOaeePasswordColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(traderTableName).Column(keaoEfkaUserNameColumnName).Exists())
            {
                Alter.Table(traderTableName)
                    .AddColumn(keaoEfkaUserNameColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(traderTableName).Column(keaoEfkaPasswordColumnName).Exists())
            {
                Alter.Table(traderTableName)
                    .AddColumn(keaoEfkaPasswordColumnName).AsString(1000).Nullable();
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
