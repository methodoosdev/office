using App.Core.Domain.Offices;
using App.Core.Domain.Traders;
using App.Core.Domain.Traders.Rating;
using App.Data.Extensions;
using App.Data.Mapping;
using FluentMigrator;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2024-04-11 00:00:00", "008 Create TraderRating - TraderInfo - TaxFactor.TaxAdvanceBCategory")]
    public class _008_Migration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(TraderRatingCategory))).Exists())
                Create.TableFor<TraderRatingCategory>();

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(TraderRating))).Exists())
                Create.TableFor<TraderRating>();

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(TraderRatingTraderMapping))).Exists())
                Create.TableFor<TraderRatingTraderMapping>();

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(TraderInfo))).Exists())
                Create.TableFor<TraderInfo>();

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(TaxFactor))).Column(nameof(TaxFactor.TaxAdvanceBCategory)).Exists())
                Alter.Table(NameCompatibilityManager.GetTableName(typeof(TaxFactor)))
                    .AddColumn(nameof(TaxFactor.TaxAdvanceBCategory)).AsInt32().NotNullable().SetExistingRowsTo(55);
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
