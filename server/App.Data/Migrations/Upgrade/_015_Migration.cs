using App.Core.Domain.Scripts;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2025-08-24 00:01:30", "015 Create Scripts")]
    public class _015_Migration : Migration
    {
        public override void Up()
        {
            if (!Schema.Table(nameof(ScriptTable)).Column(nameof(ScriptTable.Group)).Exists())
                Alter.Table(nameof(ScriptTable)).AddColumn(nameof(ScriptTable.Group))
                    .AsString().SetExistingRowsTo(null);

            if (!Schema.Table(nameof(Script)).Column(nameof(Script.Group)).Exists())
                Alter.Table(nameof(Script)).AddColumn(nameof(Script.Group))
                    .AsString().SetExistingRowsTo(null);

            if (!Schema.Table(nameof(ScriptPivot)).Column(nameof(ScriptPivot.Group)).Exists())
                Alter.Table(nameof(ScriptPivot)).AddColumn(nameof(ScriptPivot.Group))
                    .AsString().SetExistingRowsTo(null);

            if (!Schema.Table(nameof(ScriptField)).Column(nameof(ScriptField.Group)).Exists())
                Alter.Table(nameof(ScriptField)).AddColumn(nameof(ScriptField.Group))
                    .AsString().SetExistingRowsTo(null);

            if (!Schema.Table(nameof(ScriptField)).Column(nameof(ScriptField.StartingFiscalYear)).Exists())
                Alter.Table(nameof(ScriptField)).AddColumn(nameof(ScriptField.StartingFiscalYear))
                    .AsInt32().NotNullable().SetExistingRowsTo(0);

            if (!Schema.Table(nameof(ScriptField)).Column(nameof(ScriptField.Inventory)).Exists())
                Alter.Table(nameof(ScriptField)).AddColumn(nameof(ScriptField.Inventory))
                    .AsBoolean().NotNullable().SetExistingRowsTo(false);
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
