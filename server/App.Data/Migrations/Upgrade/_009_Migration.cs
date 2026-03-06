using App.Core.Domain.Employees;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2024-05-11 00:00:00", "009 Create SystemName property on Department")]
    public class _009_Migration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            //Create.Index("IX_Department_SystemName")
            //    .OnTable(nameof(Department))
            //    .OnColumn(nameof(Department.SystemName))
            //    .Ascending().WithOptions().NonClustered();

            if (!Schema.Table(nameof(Department)).Column(nameof(Department.SystemName)).Exists())
                Alter.Table(nameof(Department)).AddColumn(nameof(Department.SystemName))
                    .AsString(100).Nullable().SetExistingRowsTo(null);
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
