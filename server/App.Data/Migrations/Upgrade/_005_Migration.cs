using App.Core.Domain.Traders;
using App.Data.Extensions;
using App.Data.Mapping;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2024-02-22 00:00:00", "Trader Boardmembers", MigrationProcessType.Update)]
    public class _005_Migration : Migration 
    {
        public override void Up()
        {
            // add column
            var tableName = nameof(Trader);
            var columnName = nameof(Trader.NonRepresentationOfNaturalPerson);

            if (!Schema.Table(tableName).Column(columnName).Exists())
                Alter.Table(tableName).AddColumn(columnName).AsBoolean().NotNullable().SetExistingRowsTo(false);

            //add table
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(TraderMembership))).Exists())
                Create.TableFor<TraderMembership>();

            //add table
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(TraderRelationship))).Exists())
                Create.TableFor<TraderRelationship>();

            //add table
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(TraderBoardMemberType))).Exists())
                Create.TableFor<TraderBoardMemberType>();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
