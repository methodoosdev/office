using App.Core.Domain.Employees;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Employees
{
    public partial class JobTitleBuilder : NopEntityBuilder<JobTitle>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(JobTitle.Description)).AsString(100).NotNullable();
        }
    }
}