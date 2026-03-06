using App.Core.Domain.Offices;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Offices
{
    public partial class TaxFactorBuilder : NopEntityBuilder<TaxFactor>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }
    }
}