using App.Core.Domain.Accounting;
using App.Core.Domain.Media;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Accounting
{
    /// <summary>
    /// Represents a MyDataItem log entity builder
    /// </summary>
    public partial class MyDataItemBuilder : NopEntityBuilder<MyDataItem>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MyDataItem.TraderVat)).AsString(16).NotNullable();
        }

        #endregion
    }
}