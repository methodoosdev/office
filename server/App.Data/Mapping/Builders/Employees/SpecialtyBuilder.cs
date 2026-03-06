using App.Core.Domain.Employees;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Employees
{
    public partial class SpecialtyBuilder : NopEntityBuilder<Specialty>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Specialty.Description)).AsString(100).NotNullable();
        }
    }
}