using App.Core.Domain.Financial;
using App.Core.Domain.Messages;
using FluentMigrator;
using System;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2023-11-01 00:00:00", "FinancialObligation - EmailMessage new properties ", MigrationProcessType.Update)]
    public class _002_Migration : Migration 
    {
        public override void Up()
        {
            // add columns
            var financialObligationTableName = nameof(FinancialObligation);

            if (!Schema.Table(financialObligationTableName).Column(nameof(FinancialObligation.CustomerId)).Exists())
            {
                Alter.Table(financialObligationTableName)
                    .AddColumn(nameof(FinancialObligation.CustomerId)).AsInt32().NotNullable().SetExistingRowsTo(9878);
            }
            if (!Schema.Table(financialObligationTableName).Column(nameof(FinancialObligation.Period)).Exists())
            {
                Alter.Table(financialObligationTableName)
                    .AddColumn(nameof(FinancialObligation.Period)).AsInt32().NotNullable().SetExistingRowsTo(11);
            }
            if (!Schema.Table(financialObligationTableName).Column(nameof(FinancialObligation.CreatedOnUtc)).Exists())
            {
                Alter.Table(financialObligationTableName)
                    .AddColumn(nameof(FinancialObligation.CreatedOnUtc)).AsDateTime2().NotNullable().SetExistingRowsTo(DateTime.UtcNow);
            }

            // add columns
            var emailMessageTableName = nameof(EmailMessage);

            if (!Schema.Table(emailMessageTableName).Column(nameof(EmailMessage.SenderId)).Exists())
            {
                Alter.Table(emailMessageTableName)
                    .AddColumn(nameof(EmailMessage.SenderId)).AsInt32().NotNullable().SetExistingRowsTo(9878);
            }
            if (!Schema.Table(emailMessageTableName).Column(nameof(EmailMessage.TraderId)).Exists())
            {
                Alter.Table(emailMessageTableName)
                    .AddColumn(nameof(EmailMessage.TraderId)).AsInt32().NotNullable().SetExistingRowsTo(9878);
            }
            if (!Schema.Table(emailMessageTableName).Column(nameof(EmailMessage.Period)).Exists())
            {
                Alter.Table(emailMessageTableName)
                    .AddColumn(nameof(EmailMessage.Period)).AsInt32().NotNullable().SetExistingRowsTo(11);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
