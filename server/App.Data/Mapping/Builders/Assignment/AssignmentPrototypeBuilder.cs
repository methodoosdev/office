using App.Core.Domain.Assignment;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Assignment
{
    /// <summary>
    /// Represents a Assignment log entity builder
    /// </summary>
    public partial class AssignmentPrototypeBuilder : NopEntityBuilder<AssignmentPrototype>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AssignmentPrototype.Name)).AsString(255).NotNullable()
                .WithColumn(nameof(AssignmentPrototype.Description)).AsString(1023).Nullable();
        }

        #endregion
    }
}