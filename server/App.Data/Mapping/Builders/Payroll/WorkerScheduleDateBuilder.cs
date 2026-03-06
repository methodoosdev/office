using App.Core.Domain.Payroll;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Payroll
{
    /// <summary>
    /// Represents a WorkerScheduleDate entity builder
    /// </summary>
    public partial class WorkerScheduleDateBuilder : NopEntityBuilder<WorkerScheduleDate>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WorkerScheduleDate.WorkerScheduleId)).AsInt32().ForeignKey<WorkerSchedule>();
        }

        #endregion
    }
}