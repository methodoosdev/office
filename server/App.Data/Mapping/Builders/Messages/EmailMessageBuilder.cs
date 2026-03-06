using App.Core.Domain.Messages;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Messages
{
    /// <summary>
    /// Represents a EmailMessage log entity builder
    /// </summary>
    public partial class EmailMessageBuilder : NopEntityBuilder<EmailMessage>
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