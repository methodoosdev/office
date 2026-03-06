using App.Core.Domain.Offices;
using App.Core.Domain.Traders;
using App.Core.Domain.Traders.Rating;
using App.Data.Extensions;
using App.Data.Mapping;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2026-05-11 00:00:00", "011 Create YearCalendar - TraderYearCalendarBillingMapping")]
    public class _011_Migration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(TraderMonthlyBilling))).Exists())
                Create.TableFor<TraderMonthlyBilling>();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
