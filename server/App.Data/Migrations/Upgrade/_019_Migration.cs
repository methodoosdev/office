using App.Core.Domain.Scripts;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2025-10-15 00:00:00", "019 Create ScriptField properties")]
    public class _019_Migration : Migration
    {
        public override void Up()
        {
            if (!Schema.Table(nameof(ScriptField)).Column(nameof(ScriptField.BalanceSheet)).Exists())
                Alter.Table(nameof(ScriptField)).AddColumn(nameof(ScriptField.BalanceSheet))
                    .AsBoolean().NotNullable().SetExistingRowsTo(false);

            if (!Schema.Table(nameof(ScriptField)).Column(nameof(ScriptField.Locked)).Exists())
                Alter.Table(nameof(ScriptField)).AddColumn(nameof(ScriptField.Locked))
                    .AsBoolean().NotNullable().SetExistingRowsTo(false);
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
