using App.Core.Domain.Employees;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Employees
{
    public partial class DepartmentBuilder : NopEntityBuilder<Department>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Department.Description)).AsString(100).NotNullable();
        }
    }
}