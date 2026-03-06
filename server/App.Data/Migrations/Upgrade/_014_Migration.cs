using App.Core.Domain.Scripts;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2025-08-24 00:00:30", "014 Create ScriptFunctionTypeId")]
    public class _014_Migration : Migration
    {
        public override void Up()
        {
            if (!Schema.Table(nameof(ScriptField)).Column(nameof(ScriptField.ScriptFunctionTypeId)).Exists())
                Alter.Table(nameof(ScriptField)).AddColumn(nameof(ScriptField.ScriptFunctionTypeId))
                    .AsInt32().NotNullable().SetExistingRowsTo(0);

        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
