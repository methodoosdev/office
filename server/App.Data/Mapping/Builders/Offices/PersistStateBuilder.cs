using App.Core.Domain.Common;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Gdpr
{
    /// <summary>
    /// Represents a PersistState log entity builder
    /// </summary>
    public partial class PersistStateBuilder : NopEntityBuilder<PersistState>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PersistState.ModelType)).AsString(255).NotNullable();
        }

        #endregion
    }
}