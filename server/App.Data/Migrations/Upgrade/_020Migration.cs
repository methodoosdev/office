using App.Core.Domain.Payroll;
using App.Data.Extensions;
using App.Data.Mapping;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2025-11-13 00:00:00", "020 Create ScriptTool")]
    public class _020Migration : Migration
    {
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ApdTeka))).Exists())
                Create.TableFor<ApdTeka>();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
