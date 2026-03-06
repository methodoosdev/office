using App.Core.Domain.Scripts;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2025-10-05 00:00:00", "018 Create new properties ScriptTool")]
    public class _018Migration : Migration
    {
        public override void Up()
        {
            if (!Schema.Table(nameof(Script)).Column(nameof(Script.Replacement)).Exists())
                Alter.Table(nameof(Script)).AddColumn(nameof(Script.Replacement))
                    .AsString().NotNullable().SetExistingRowsTo("#test");

            if (!Schema.Table(nameof(ScriptTool)).Column(nameof(ScriptTool.FileName)).Exists())
                Alter.Table(nameof(ScriptTool)).AddColumn(nameof(ScriptTool.FileName))
                    .AsString(255).Nullable();

            if (!Schema.Table(nameof(ScriptTool)).Column(nameof(ScriptTool.Extension)).Exists())
                Alter.Table(nameof(ScriptTool)).AddColumn(nameof(ScriptTool.Extension))
                    .AsString(10).Nullable();

            if (!Schema.Table(nameof(ScriptTool)).Column(nameof(ScriptTool.ContentType)).Exists())
                Alter.Table(nameof(ScriptTool)).AddColumn(nameof(ScriptTool.ContentType))
                    .AsString(150).Nullable();

            if (!Schema.Table(nameof(ScriptTool)).Column(nameof(ScriptTool.SizeBytes)).Exists())
                Alter.Table(nameof(ScriptTool)).AddColumn(nameof(ScriptTool.SizeBytes))
                    .AsInt64().Nullable();

            if (!Schema.Table(nameof(ScriptTool)).Column(nameof(ScriptTool.Bytes)).Exists())
                Alter.Table(nameof(ScriptTool)).AddColumn(nameof(ScriptTool.Bytes))
                    .AsBinary(int.MaxValue).Nullable();

            if (!Schema.Table(nameof(ScriptTool)).Column(nameof(ScriptTool.CreatedOnUtc)).Exists())
                Alter.Table(nameof(ScriptTool)).AddColumn(nameof(ScriptTool.CreatedOnUtc))
                    .AsDateTime2().Nullable();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
