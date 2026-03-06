using App.Core.Domain.Scripts;
using App.Data.Extensions;
using App.Data.Mapping;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2025-08-17 00:00:00", "012 Story")]
    public class _012_Migration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ScriptTableName))).Exists())
                Create.TableFor<ScriptTableName>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ScriptTable))).Exists())
                Create.TableFor<ScriptTable>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ScriptTableItem))).Exists())
                Create.TableFor<ScriptTableItem>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ScriptField))).Exists())
                Create.TableFor<ScriptField>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Script))).Exists())
                Create.TableFor<Script>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ScriptItem))).Exists())
                Create.TableFor<ScriptItem>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ScriptPivot))).Exists())
                Create.TableFor<ScriptPivot>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ScriptPivotItem))).Exists())
                Create.TableFor<ScriptPivotItem>();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
