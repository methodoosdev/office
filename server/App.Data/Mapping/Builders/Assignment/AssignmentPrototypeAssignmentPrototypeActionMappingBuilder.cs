using App.Core.Domain.Assignment;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Assignment
{
    /// <summary>
    /// Represents a Assignment log entity builder
    /// </summary>
    public partial class AssignmentPrototypeAssignmentPrototypeActionMappingBuilder : NopEntityBuilder<AssignmentPrototypeAssignmentPrototypeActionMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(AssignmentPrototypeAssignmentPrototypeActionMapping), nameof(AssignmentPrototypeAssignmentPrototypeActionMapping.AssignmentPrototypeId)))
                    .AsInt32().ForeignKey<AssignmentPrototype>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(AssignmentPrototypeAssignmentPrototypeActionMapping), nameof(AssignmentPrototypeAssignmentPrototypeActionMapping.AssignmentPrototypeActionId)))
                    .AsInt32().ForeignKey<AssignmentPrototypeAction>().PrimaryKey();

        }

        #endregion
    }
}