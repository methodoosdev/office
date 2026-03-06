using App.Core.Domain.SimpleTask;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Blogs
{
    /// <summary>
    /// Represents a blog post entity builder
    /// </summary>
    public partial class SimpleTaskManagerBuilder : NopEntityBuilder<SimpleTaskManager>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(SimpleTaskManager.Name)).AsString(200).NotNullable()
                .WithColumn(nameof(SimpleTaskManager.StartingDate)).AsDateTime2().Nullable()
                .WithColumn(nameof(SimpleTaskManager.EndingDate)).AsDateTime2().Nullable()
                .WithColumn(nameof(SimpleTaskManager.CreatedDate)).AsDateTime2().Nullable()
                .WithColumn(nameof(SimpleTaskManager.SimpleTaskCategoryId)).AsInt32().Nullable()
                    .ForeignKey<SimpleTaskCategory>(onDelete: System.Data.Rule.SetNull)
                .WithColumn(nameof(SimpleTaskManager.SimpleTaskDepartmentId)).AsInt32().Nullable()
                    .ForeignKey<SimpleTaskDepartment>(onDelete: System.Data.Rule.SetNull)
                .WithColumn(nameof(SimpleTaskManager.SimpleTaskNatureId)).AsInt32().Nullable()
                    .ForeignKey<SimpleTaskNature>(onDelete: System.Data.Rule.SetNull)
                .WithColumn(nameof(SimpleTaskManager.SimpleTaskSectorId)).AsInt32().Nullable()
                    .ForeignKey<SimpleTaskSector>(onDelete: System.Data.Rule.SetNull);
        }

        #endregion
    }
}