using App.Core.Domain.Customers;
using App.Core.Domain.Scripts;
using App.Core.Domain.Traders;
using App.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using System.Data;

namespace App.Data.Mapping.Builders.Scripts
{
    public partial class ScriptTableNameBuilder : NopEntityBuilder<ScriptTableName>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScriptTableName.Name)).AsString(1000).Nullable();
        }
    }
    public partial class ScriptTableBuilder : NopEntityBuilder<ScriptTable>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScriptTable.TableName)).AsString(1000).Nullable()
                .WithColumn(nameof(ScriptTable.TraderId)).AsInt32().ForeignKey<Trader>();
        }
    }
    public partial class ScriptGroupBuilder : NopEntityBuilder<ScriptGroup>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScriptGroup.GroupName)).AsString(1000).Nullable()
                .WithColumn(nameof(ScriptTable.TraderId)).AsInt32().ForeignKey<Trader>();
        }
    }
    public partial class ScriptTableItemBuilder : NopEntityBuilder<ScriptTableItem>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScriptTableItem.AccountingCode)).AsString(100).Nullable()
                .WithColumn(nameof(ScriptTableItem.ScriptTableId)).AsInt32().ForeignKey<ScriptTable>();
        }
    }
    public partial class ScriptFieldBuilder : NopEntityBuilder<ScriptField>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScriptField.FieldName)).AsString(1000).Nullable()
                .WithColumn(nameof(ScriptField.TraderId)).AsInt32().ForeignKey<Trader>();
        }
    }
    public partial class ScriptBuilder : NopEntityBuilder<Script>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Script.ScriptName)).AsString(1000).Nullable()
                .WithColumn(nameof(Script.Description)).AsString(4000).Nullable()
                .WithColumn(nameof(Script.TraderId)).AsInt32().ForeignKey<Trader>();
        }
    }
    public partial class ScriptItemBuilder : NopEntityBuilder<ScriptItem>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScriptItem.ScriptId)).AsInt32().ForeignKey<Script>();
        }
    }
    public partial class ScriptPivotBuilder : NopEntityBuilder<ScriptPivot>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScriptPivot.ScriptPivotName)).AsString(1000).Nullable()
                .WithColumn(nameof(ScriptPivot.Description)).AsString(4000).Nullable()
                .WithColumn(nameof(ScriptPivot.TraderId)).AsInt32().ForeignKey<Trader>();
        }
    }
    public partial class ScriptPivotItemBuilder : NopEntityBuilder<ScriptPivotItem>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScriptPivotItem.ScriptPivotId)).AsInt32().ForeignKey<ScriptPivot>();
        }
    }

    public partial class ScriptToolBuilder : NopEntityBuilder<ScriptTool>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScriptTool.Title)).AsString(1000).Nullable()
                .WithColumn(nameof(ScriptTool.Subtitle)).AsString(1000).Nullable()
                .WithColumn(nameof(ScriptTool.Description)).AsString(4000).Nullable()
                .WithColumn(nameof(ScriptTool.TraderId)).AsInt32().ForeignKey<Trader>();
        }
    }
    public partial class ScriptToolItemBuilder : NopEntityBuilder<ScriptToolItem>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScriptToolItem.ScriptToolId)).AsInt32().ForeignKey<ScriptTool>();
        }
    }
}