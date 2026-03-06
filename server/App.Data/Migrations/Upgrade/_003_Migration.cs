using App.Core.Domain.Accounting;
using App.Data.Extensions;
using App.Data.Mapping;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2024-01-01 00:00:00", "Create MyDataItem")]
    public class _003_Migration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(MyDataItem))).Exists())
            {
                Create.TableFor<MyDataItem>();
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
