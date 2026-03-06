using App.Core.Domain.Accounting;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2024-02-06 00:00:00", "MyDataItem new property Series", MigrationProcessType.Update)]
    public class _004_Migration : Migration 
    {
        public override void Up()
        {
            // add column
            var tableName = nameof(MyDataItem);
            var columnName = nameof(MyDataItem.Series);

            if (!Schema.Table(tableName).Column(columnName).Exists())
            {
                Alter.Table(tableName)
                    .AddColumn(columnName).AsString(100).NotNullable();
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
