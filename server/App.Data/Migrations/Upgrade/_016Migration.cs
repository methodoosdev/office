using App.Core.Domain.Scripts;
using App.Data.Extensions;
using App.Data.Mapping;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2025-09-04 00:00:00", "016 Create ScriptGroup")]
    public class _016Migration : Migration
    {
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ScriptGroup))).Exists())
                Create.TableFor<ScriptGroup>();

            if (!Schema.Table(nameof(Script)).Column(nameof(Script.ScriptGroupId)).Exists())
                Alter.Table(nameof(Script)).AddColumn(nameof(Script.ScriptGroupId))
                    .AsInt32().NotNullable().SetExistingRowsTo(0);

            if (!Schema.Table(nameof(ScriptField)).Column(nameof(ScriptField.ScriptGroupId)).Exists())
                Alter.Table(nameof(ScriptField)).AddColumn(nameof(ScriptField.ScriptGroupId))
                    .AsInt32().NotNullable().SetExistingRowsTo(0);

            if (!Schema.Table(nameof(ScriptPivot)).Column(nameof(ScriptPivot.ScriptGroupId)).Exists())
                Alter.Table(nameof(ScriptPivot)).AddColumn(nameof(ScriptPivot.ScriptGroupId))
                    .AsInt32().NotNullable().SetExistingRowsTo(0);

            if (!Schema.Table(nameof(ScriptTable)).Column(nameof(ScriptTable.ScriptGroupId)).Exists())
                Alter.Table(nameof(ScriptTable)).AddColumn(nameof(ScriptTable.ScriptGroupId))
                    .AsInt32().NotNullable().SetExistingRowsTo(0);
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
