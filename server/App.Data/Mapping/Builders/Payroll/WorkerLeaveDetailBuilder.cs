using App.Core.Domain.Payroll;
using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Blogs
{
    /// <summary>
    /// Represents a WorkerLeaveDetail entity builder
    /// </summary>
    public partial class WorkerLeaveDetailBuilder : NopEntityBuilder<WorkerLeaveDetail>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WorkerLeaveDetail.TraderId)).AsInt32().ForeignKey<Trader>();
        }

        #endregion
    }
}