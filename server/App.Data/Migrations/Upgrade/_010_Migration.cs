using App.Core.Domain.Employees;
using App.Core.Domain.Traders;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2024-12-11 00:00:00", "010 Create rating properties")]
    public class _010_Migration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!Schema.Table(nameof(Trader)).Column(nameof(Trader.TraderPayment)).Exists())
                Alter.Table(nameof(Trader)).AddColumn(nameof(Trader.TraderPayment))
                    .AsInt32().SetExistingRowsTo(0m);

            if (!Schema.Table(nameof(Employee)).Column(nameof(Employee.EmployeeSalary)).Exists())
                Alter.Table(nameof(Employee)).AddColumn(nameof(Employee.EmployeeSalary))
                    .AsInt32().SetExistingRowsTo(0m);
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
