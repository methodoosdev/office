using App.Core.Domain.Traders;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2024-04-01 00:00:00", "006 Modify Trader properties")]
    public class _006_Migration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (Schema.Table(nameof(Trader)).Column(nameof(Trader.Vat)).Exists())
                Alter.Table(nameof(Trader)).AlterColumn(nameof(Trader.Vat)).AsString(1000).NotNullable();

            if (Schema.Table(nameof(Trader)).Column(nameof(Trader.LastName)).Exists())
                Alter.Table(nameof(Trader)).AlterColumn(nameof(Trader.LastName)).AsString(1000).Nullable();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
