using App.Core.Domain.Traders;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2026-01-13 00:00:00", "010 Create rating properties")]
    public class _022_Migration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!Schema.Table(nameof(Trader)).Column(nameof(Trader.TraderExpense)).Exists())
                Alter.Table(nameof(Trader)).AddColumn(nameof(Trader.TraderExpense))
                    .AsInt32().SetExistingRowsTo(0m);
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
