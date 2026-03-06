using App.Core.Domain.Traders;
using App.Core.Domain.VatExemption;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Blogs
{
    /// <summary>
    /// Represents a blog post entity builder
    /// </summary>
    public partial class VatExemptionDocBuilder : NopEntityBuilder<VatExemptionDoc>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(VatExemptionDoc.CreatedDate)).AsDateTime2().Nullable()
                .WithColumn(nameof(VatExemptionDoc.ApprovalExpiryDate)).AsDateTime2()
                .WithColumn(nameof(VatExemptionDoc.TraderId)).AsInt32().ForeignKey<Trader>();
        }

        #endregion
    }
}