using App.Core.Domain.Employees;
using App.Core.Domain.Offices;
using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Traders
{
    public partial class TraderEmployeeMappingBuilder : NopEntityBuilder<TraderEmployeeMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TraderEmployeeMapping), nameof(TraderEmployeeMapping.TraderId)))
                    .AsInt32().ForeignKey<Trader>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TraderEmployeeMapping), nameof(TraderEmployeeMapping.EmployeeId)))
                    .AsInt32().ForeignKey<Employee>().PrimaryKey();
        }
    }
}
