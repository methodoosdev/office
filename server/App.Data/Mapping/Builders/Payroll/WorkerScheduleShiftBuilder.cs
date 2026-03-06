using App.Core.Domain.Payroll;
using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Payroll
{
    /// <summary>
    /// Represents a WorkerScheduleShift entity builder
    /// </summary>
    public partial class WorkerScheduleShiftBuilder : NopEntityBuilder<WorkerScheduleShift>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WorkerScheduleShift.TraderId)).AsInt32().ForeignKey<Trader>();
        }

        #endregion
    }
}