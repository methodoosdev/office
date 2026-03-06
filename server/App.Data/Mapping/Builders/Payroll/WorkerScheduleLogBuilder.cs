using App.Core.Domain.Payroll;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Payroll
{
    /// <summary>
    /// Represents a WorkerScheduleLog entity builder
    /// </summary>
    public partial class WorkerScheduleLogBuilder : NopEntityBuilder<WorkerScheduleLog>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WorkerScheduleLog.WorkerScheduleId)).AsInt32().ForeignKey<WorkerSchedule>();
        }

        #endregion
    }
}