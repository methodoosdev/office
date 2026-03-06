using App.Core.Domain.Accounting;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Accounting
{
    /// <summary>
    /// Represents a PeriodicF2 log entity builder
    /// </summary>
    public partial class PeriodicF2Builder : NopEntityBuilder<PeriodicF2>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }

        #endregion
    }
}