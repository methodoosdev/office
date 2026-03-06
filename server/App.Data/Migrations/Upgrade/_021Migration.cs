using App.Core.Domain.Payroll;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2025-11-22 00:00:00", "021 Create new properties ApdTeka")]
    public class _021Migration : Migration
    {
        public override void Up()
        {
            if (!Schema.Table(nameof(ApdTeka)).Column(nameof(ApdTeka.DxApd)).Exists())
                Alter.Table(nameof(ApdTeka)).AddColumn(nameof(ApdTeka.DxApd))
                    .AsDecimal(12, 2).NotNullable().SetExistingRowsTo(0m);

            if (!Schema.Table(nameof(ApdTeka)).Column(nameof(ApdTeka.DxTeka)).Exists())
                Alter.Table(nameof(ApdTeka)).AddColumn(nameof(ApdTeka.DxTeka))
                    .AsDecimal(12, 2).NotNullable().SetExistingRowsTo(0m);

            if (!Schema.Table(nameof(ApdTeka)).Column(nameof(ApdTeka.DpApd)).Exists())
                Alter.Table(nameof(ApdTeka)).AddColumn(nameof(ApdTeka.DpApd))
                    .AsDecimal(12, 2).NotNullable().SetExistingRowsTo(0m);

            if (!Schema.Table(nameof(ApdTeka)).Column(nameof(ApdTeka.DpTeka)).Exists())
                Alter.Table(nameof(ApdTeka)).AddColumn(nameof(ApdTeka.DpTeka))
                    .AsDecimal(12, 2).NotNullable().SetExistingRowsTo(0m);


            if (!Schema.Table(nameof(ApdTeka)).Column(nameof(ApdTeka.WorkersError)).Exists())
                Alter.Table(nameof(ApdTeka)).AddColumn(nameof(ApdTeka.WorkersError))
                    .AsString().Nullable();

            if (!Schema.Table(nameof(ApdTeka)).Column(nameof(ApdTeka.ApdError)).Exists())
                Alter.Table(nameof(ApdTeka)).AddColumn(nameof(ApdTeka.ApdError))
                    .AsString().Nullable();

            if (!Schema.Table(nameof(ApdTeka)).Column(nameof(ApdTeka.TekaError)).Exists())
                Alter.Table(nameof(ApdTeka)).AddColumn(nameof(ApdTeka.TekaError))
                    .AsString().Nullable();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
