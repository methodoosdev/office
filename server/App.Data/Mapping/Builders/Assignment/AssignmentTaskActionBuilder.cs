using App.Core.Domain.Assignment;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Assignment
{
    /// <summary>
    /// Represents a Assignment log entity builder
    /// </summary>
    public partial class AssignmentTaskActionBuilder : NopEntityBuilder<AssignmentTaskAction>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AssignmentTaskAction.ActionName)).AsString(255).NotNullable()
                .WithColumn(nameof(AssignmentTaskAction.ActionDescription)).AsString(1023).Nullable()
                .WithColumn(nameof(AssignmentTaskAction.AssignmentTaskId)).AsInt32().ForeignKey<AssignmentTask>();
        }

        #endregion
    }
}