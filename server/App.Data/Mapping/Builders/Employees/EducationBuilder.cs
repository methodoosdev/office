using App.Core.Domain.Employees;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Employees
{
    public partial class EducationBuilder : NopEntityBuilder<Education>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Education.Description)).AsString(100).NotNullable();
        }
    }
}