using App.Core.Domain.Scripts;
using App.Data.Extensions;
using App.Data.Mapping;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2025-09-04 01:01:01", "017 Create ScriptTool")]
    public class _017Migration : Migration
    {
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ScriptTool))).Exists())
                Create.TableFor<ScriptTool>();

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ScriptToolItem))).Exists())
                Create.TableFor<ScriptToolItem>();

            if (!Schema.Table(nameof(ScriptGroup)).Column(nameof(ScriptGroup.ScriptAlignTypeId)).Exists())
                Alter.Table(nameof(ScriptGroup)).AddColumn(nameof(ScriptGroup.ScriptAlignTypeId))
                    .AsInt32().NotNullable().SetExistingRowsTo(0);

            if (!Schema.Table(nameof(Script)).Column(nameof(Script.ScriptAlignTypeId)).Exists())
                Alter.Table(nameof(Script)).AddColumn(nameof(Script.ScriptAlignTypeId))
                    .AsInt32().NotNullable().SetExistingRowsTo(0);

            if (!Schema.Table(nameof(Script)).Column(nameof(Script.ScriptCode)).Exists())
                Alter.Table(nameof(Script)).AddColumn(nameof(Script.ScriptCode))
                    .AsString().SetExistingRowsTo(null);

            if (!Schema.Table(nameof(Script)).Column(nameof(Script.HasHeader)).Exists())
                Alter.Table(nameof(Script)).AddColumn(nameof(Script.HasHeader))
                    .AsBoolean().NotNullable().SetExistingRowsTo(false);

            if (!Schema.Table(nameof(Script)).Column(nameof(Script.HeaderCode)).Exists())
                Alter.Table(nameof(Script)).AddColumn(nameof(Script.HeaderCode))
                    .AsString().SetExistingRowsTo(null);

            if (!Schema.Table(nameof(Script)).Column(nameof(Script.Header)).Exists())
                Alter.Table(nameof(Script)).AddColumn(nameof(Script.Header))
                    .AsString().SetExistingRowsTo(null);

            if (!Schema.Table(nameof(Script)).Column(nameof(Script.HeaderLeft)).Exists())
                Alter.Table(nameof(Script)).AddColumn(nameof(Script.HeaderLeft))
                    .AsString().SetExistingRowsTo(null);

            if (!Schema.Table(nameof(Script)).Column(nameof(Script.HeaderRight)).Exists())
                Alter.Table(nameof(Script)).AddColumn(nameof(Script.HeaderRight))
                    .AsString().SetExistingRowsTo(null);
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
