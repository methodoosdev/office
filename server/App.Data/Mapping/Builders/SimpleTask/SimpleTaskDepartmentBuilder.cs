using App.Core.Domain.SimpleTask;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Blogs
{
    /// <summary>
    /// Represents a blog post entity builder
    /// </summary>
    public partial class SimpleTaskDepartmentBuilder : NopEntityBuilder<SimpleTaskDepartment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(SimpleTaskDepartment.Description)).AsString(100).NotNullable();
        }

        #endregion
    }
}