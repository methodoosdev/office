using App.Core.Domain.Payroll;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Blogs
{
    /// <summary>
    /// Represents a ApdTeka entity builder
    /// </summary>
    public partial class ApdTekaBuilder : NopEntityBuilder<ApdTeka>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ApdTeka.ApdSubmitDateOnUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(ApdTeka.TekaSubmitDateOnUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(ApdTeka.InfoDateOnUtc)).AsDateTime2().Nullable();
        }

        #endregion
    }
}