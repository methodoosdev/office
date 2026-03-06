using App.Core.Domain.Scripts;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2025-08-24 00:00:00", "013 Create Scripts properties")]
    public class _013_Migration : Migration
    {
        public override void Up()
        {
            if (!Schema.Table(nameof(ScriptField)).Column(nameof(ScriptField.ScriptQueryTypeId)).Exists())
                Alter.Table(nameof(ScriptField)).AddColumn(nameof(ScriptField.ScriptQueryTypeId))
                    .AsInt32().NotNullable().SetExistingRowsTo(0);

            if (!Schema.Table(nameof(ScriptTableItem)).Column(nameof(ScriptTableItem.ScriptBehaviorTypeId)).Exists())
                Alter.Table(nameof(ScriptTableItem)).AddColumn(nameof(ScriptTableItem.ScriptBehaviorTypeId))
                    .AsInt32().NotNullable().SetExistingRowsTo(0);

            if (!Schema.Table(nameof(ScriptPivotItem)).Column(nameof(ScriptPivotItem.Printed)).Exists())
                Alter.Table(nameof(ScriptPivotItem)).AddColumn(nameof(ScriptPivotItem.Printed))
                    .AsBoolean().NotNullable().SetExistingRowsTo(false);

        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
