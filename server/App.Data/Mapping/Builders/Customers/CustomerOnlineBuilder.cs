using App.Core.Domain.Customers;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Gdpr
{
    /// <summary>
    /// Represents a CustomerOnline log entity builder
    /// </summary>
    public partial class CustomerOnlineBuilder : NopEntityBuilder<CustomerOnline>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }

        #endregion
    }
}