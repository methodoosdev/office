using App.Core.Domain.Employees;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using System.Data;

namespace App.Data.Mapping.Builders.Employees
{
    public partial class EmployeeBuilder : NopEntityBuilder<Employee>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Employee.FirstName)).AsString(50).NotNullable()
                .WithColumn(nameof(Employee.LastName)).AsString(150).NotNullable()
                .WithColumn(nameof(Employee.EmailContact)).AsString(150).NotNullable();
        }
    }
}