using App.Core;
using App.Core.Domain.Scripts;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Scripts;
using App.Models.Traders;
using App.Services;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Scripts;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptTraderModelFactory
    {
        Task<TraderSearchModel> PrepareScriptTraderSearchModelAsync(TraderSearchModel searchModel);
        Task<TraderListModel> PrepareScriptTraderListModelAsync(TraderSearchModel searchModel);
        Task<TraderModel> PrepareTraderModelAsync(TraderModel model, Trader trader);
        Task<TraderFormModel> PrepareScriptTraderFormModelAsync(TraderFormModel formModel);
        Task<object> PrepareDiagramAsync(int scriptToolId, int traderId);
        Task<(string Title, List<ScriptReport> Data)> PrepareScriptToolAsync(int scriptToolId, int traderId, ScriptToolConfigModel config);
        Task<GridResponse<DynamicModel>> PrepareScriptReportAsync(IList<string> groups, int traderId, int categoryBookTypeId, string traderName, ScriptToolConfigModel config);
        Task<PivotResponse> PrepareScriptPivotAsync(ScriptTraderModel model);
    }
    public partial class ScriptTraderModelFactory : IScriptTraderModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly IScriptFieldService _scriptFieldService;
        private readonly IScriptGroupService _scriptGroupService;
        private readonly IScriptService _scriptService;
        private readonly IScriptItemService _scriptItemService;
        private readonly IScriptPivotService _scriptPivotService;
        private readonly IScriptPivotItemService _scriptPivotItemService;
        private readonly IScriptTraderModelService _scriptTraderModelService;
        private readonly IScriptToolService _scriptToolService;
        private readonly IScriptToolItemService _scriptToolItemService;
        private readonly IScriptTableService _scriptTableService;
        private readonly IScriptTableItemService _scriptTableItemService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptTraderModelFactory(ITraderService traderService,
            IScriptFieldService scriptFieldService,
            IScriptGroupService scriptGroupService,
            IScriptService scriptService,
            IScriptItemService scriptItemService,
            IScriptPivotService scriptPivotService,
            IScriptPivotItemService scriptPivotItemService,
            IScriptTraderModelService scriptTraderModelService,
            IScriptToolService scriptToolService,
            IScriptToolItemService scriptToolItemService,
            IScriptTableService scriptTableService,
            IScriptTableItemService scriptTableItemService,
            ITraderConnectionService traderConnectionService,
            IFieldConfigService fieldConfigService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _scriptFieldService = scriptFieldService;
            _scriptGroupService = scriptGroupService;
            _scriptService = scriptService;
            _scriptItemService = scriptItemService;
            _scriptPivotService = scriptPivotService;
            _scriptPivotItemService = scriptPivotItemService;
            _scriptTraderModelService = scriptTraderModelService;
            _scriptToolService = scriptToolService;
            _scriptToolItemService = scriptToolItemService;
            _scriptTableService = scriptTableService;
            _scriptTableItemService = scriptTableItemService;
            _traderConnectionService = traderConnectionService;
            _fieldConfigService = fieldConfigService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<TraderSearchModel> PrepareScriptTraderSearchModelAsync(TraderSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderListModel> PrepareScriptTraderListModelAsync(TraderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));


            var query = _traderService.Table.AsEnumerable().Select(x =>
            {
                var model = x.ToModel<TraderModel>();

                model.FullName = model.FullName() ?? "";
                model.Vat = model.Vat ?? "";
                model.Doy = model.Doy ?? "";
                model.Email = model.Email ?? "";

                return model;
            }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.FullName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Vat.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Doy.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Email.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.Where(c =>
                c.CategoryBookTypeId == (int)CategoryBookType.C ||
                c.CategoryBookTypeId == (int)CategoryBookType.B);

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            var traders = await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);

            //prepare grid model
            var model = new TraderListModel().PrepareToGrid(searchModel, traders);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderModel>(1, nameof(TraderModel.FullName), ColumnType.RouterLink),
                ColumnConfig.Create<TraderModel>(2, nameof(TraderModel.Vat)),
                ColumnConfig.Create<TraderModel>(2, nameof(TraderModel.Email)),
                ColumnConfig.Create<TraderModel>(2, nameof(TraderModel.ConnectionAccountingActive), ColumnType.Checkbox, filterType: "boolean"),
            };

            return columns;
        }

        public virtual Task<TraderModel> PrepareTraderModelAsync(TraderModel model, Trader trader)
        {
            if (trader != null)
            {
                //fill in model values from the entity
                model ??= trader.ToModel<TraderModel>();
                model.TaxSystemId = 0;
            }

            return Task.FromResult(model);
        }

        public virtual async Task<TraderFormModel> PrepareScriptTraderFormModelAsync(TraderFormModel formModel)
        {
            var lookup = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<TraderModel>(
                    nameof(TraderModel.TaxSystemId), FieldConfigType.WithCategoryBooks, hideLabel: true)
            };

            var top = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LastName), FieldType.Text, _readonly: true, hideLabel: true, className: "col-12 md:col-6"),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Vat), FieldType.Text, _readonly: true, hideLabel: true, className: "col-12 md:col-3"),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ConnectionAccountingActive), FieldType.Checkbox, disabled: true, hideLabel: true, className: "col-12 md:col-3")
            };

            var bottom = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>("cloneScripts", FieldType.Button, themeColor: "primary",
                className: "text-right p-3", disableExpression: "!(model.taxSystemId > 0)", 
                label: await _localizationService.GetResourceAsync("App.Common.Copy")),
                FieldConfig.Create<TraderModel>("deleteScripts", FieldType.Button, themeColor: "error",
                className: "text-right p-3",
                label: await _localizationService.GetResourceAsync("App.Common.DeleteAll"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12", "col-12 md:col-6", "col-12 md:col-3" }, top, lookup, bottom);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        public virtual async Task<object> PrepareDiagramAsync(int scriptToolId, int traderId)
        {
            var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(scriptToolId);
            var scriptToolItems = await _scriptToolItemService.GetAllScriptToolItemsAsync(scriptTool.Id);
            var scriptIds = scriptToolItems.Select(x => x.ScriptId).ToArray();
            var scripts = _scriptService.Table.AsEnumerable()
                .Where(x => scriptIds.Contains(x.Id))
                .Select(x => 
                {
                    var scriptGroup = _scriptGroupService.Table.FirstOrDefault(k => k.Id == x.ScriptGroupId);

                    var model = x.ToModel<ScriptModel>();

                    model.ScriptGroupName = scriptGroup?.GroupName ?? "No Group";
                    model.Order = scriptGroup?.Order ?? 0;

                    return model;
                })
                .ToList();

            var scriptItems = new List<ScriptItemModel>();
            var scriptTypes = await ScriptType.Field.ToSelectionItemListAsync();

            foreach (var script in scripts)
            {
                var scrItems = (await _scriptItemService.GetAllScriptItemsAsync(script.Id))
                    .Select(x =>
                    {
                        var scriptField = _scriptFieldService.Table.FirstOrDefault(k => k.Id == x.ScriptFieldId);
                        
                        var model = x.ToModel<ScriptItemModel>();

                        model.ScriptName = script.ScriptName;
                        model.ParentGroupName = script.ScriptGroupName;

                        model.ScriptTypeName = scriptTypes.FirstOrDefault(a => a.Value == x.ScriptTypeId)?.Label ?? "";
                        model.ParentName =
                            x.ScriptTypeId == (int)ScriptType.Script ? script?.ScriptName ?? "" : scriptField?.FieldName ?? "";

                        return model;
                    }).ToList();

                scriptItems.AddRange(scrItems);
            }

            var functions = await ScriptFunctionType.TaxesFee.ToSelectionItemListAsync();
            var fieldTypes = await ScriptFieldType.Table.ToSelectionItemListAsync();
            var queries = await ScriptQueryType.Payments.ToSelectionItemListAsync();

            var scriptFieldIds = scriptItems.Where(x => x.ScriptFieldId > 0)
                .Select(x => x.ScriptFieldId.HasValue ? x.ScriptFieldId.Value : 0).ToArray();
            var scrFields = await _scriptFieldService.GetScriptFieldsByIdsAsync(scriptFieldIds);

            var scriptFields = scrFields.AsEnumerable()
                .Select(x => 
                {
                    var model = x.ToModel<ScriptFieldModel>();

                    model.ScriptFieldTypeName = fieldTypes.FirstOrDefault(a => a.Value == x.ScriptFieldTypeId)?.Label ?? "";

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Table)
                    {
                        var codes = _scriptTableItemService.Table
                            .Where(k => k.ScriptTableId == x.ScriptTableId)
                            .Select(k => k.AccountingCode)
                            .ToArray();

                        model.ScriptDetailName = string.Join(" , ", codes);
                    }

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Query)
                        model.ScriptDetailName = queries.FirstOrDefault(a => a.Value == x.ScriptQueryTypeId)?.Label ?? "";

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Function)
                        model.ScriptDetailName = functions.FirstOrDefault(a => a.Value == x.ScriptFunctionTypeId)?.Label ?? "";

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Fixed)
                        model.ScriptDetailName = x.FixedValue.ToString("n2");

                    return model;
                }).ToList();

            return new
            {
                rootTool = scriptTool.ToModel<ScriptToolModel>(),
                scriptToolItems = scriptToolItems.Select(x => x.ToModel<ScriptToolItemModel>()).ToList(),
                scripts,
                scriptItems,
                scriptFields

            };
        }
        public virtual async Task<object> PrepareDiagramAsync2(int scriptToolId, int traderId)
        {
            var scriptTypes = await ScriptType.Field.ToSelectionItemListAsync();
            var functions = await ScriptFunctionType.TaxesFee.ToSelectionItemListAsync();
            var fieldTypes = await ScriptFieldType.Table.ToSelectionItemListAsync();
            var queries = await ScriptQueryType.Payments.ToSelectionItemListAsync();
            var scriptGroups = await _scriptGroupService.GetAllScriptGroupsAsync(traderId);

            var rootTool = await _scriptToolService.GetScriptToolByIdAsync(scriptToolId);
            var toolItems = await _scriptToolItemService.GetAllScriptToolItemsAsync(rootTool.Id);
            var scriptIds = toolItems.Select(x => x.ScriptId).ToArray();
            var scriptItems = await _scriptItemService.Table.Where(x => scriptIds.Contains(x.ScriptId)).ToListAsync();

            var scriptItemList = scriptItems
                .Select(x =>
                {
                    var script = _scriptService.Table.FirstOrDefault(k => k.Id == x.ParentId);
                    var scriptField = _scriptFieldService.Table.FirstOrDefault(k => k.Id == x.ScriptFieldId);
                    var scriptGroupName = script == null ? "" : scriptGroups
                        .FirstOrDefault(k => k.Id == script.ScriptGroupId)?.GroupName ?? "";
                    var fieldGroupName = scriptField == null ? "" : scriptGroups
                        .FirstOrDefault(k => k.Id == scriptField.ScriptGroupId)?.GroupName ?? "";

                    var model = x.ToModel<ScriptItemModel>();

                    model.ScriptName = script?.ScriptName;
                    model.ParentGroupName =
                        x.ScriptTypeId == (int)ScriptType.Script ? scriptGroupName : fieldGroupName;
                    model.ScriptTypeName = scriptTypes.FirstOrDefault(a => a.Value == x.ScriptTypeId)?.Label ?? "";
                    model.ParentName =
                        x.ScriptTypeId == (int)ScriptType.Script ? script?.ScriptName ?? "" : scriptField?.FieldName ?? "";

                    return model;
                })
                .ToList();

            var scriptFieldIds = scriptItems.Where(x => x.ScriptFieldId > 0).Select(x => x.ScriptFieldId).ToArray();
            var scriptFields = await _scriptFieldService.GetScriptFieldsByIdsAsync(scriptFieldIds);

            var scriptFieldList = scriptFields
                .Select(x =>
                {
                    var model = x.ToModel<ScriptFieldModel>();

                    model.ScriptFieldTypeName = fieldTypes.FirstOrDefault(a => a.Value == x.ScriptFieldTypeId)?.Label ?? "";

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Table)
                    {
                        var codes = _scriptTableItemService.Table
                            .Where(k => k.ScriptTableId == x.ScriptTableId)
                            .Select(k => k.AccountingCode)
                            .ToArray();

                        model.ScriptDetailName = string.Join(" , ", codes);
                    }

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Query)
                        model.ScriptDetailName = queries.FirstOrDefault(a => a.Value == x.ScriptQueryTypeId)?.Label ?? "";

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Function)
                        model.ScriptDetailName = functions.FirstOrDefault(a => a.Value == x.ScriptFunctionTypeId)?.Label ?? "";

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Fixed)
                        model.ScriptDetailName = x.FixedValue.ToString("n2");

                    return model;
                })
                .ToList();

            return new
            {
                rootTool = rootTool.ToModel<ScriptToolModel>(),
                toolItems = toolItems.Select(x => x.ToModel<ScriptToolItemModel>()).ToList(),
                scriptItems = scriptItemList,
                scriptFields = scriptFieldList
            };

        }

        public virtual async Task<(string Title, List<ScriptReport> Data)> PrepareScriptToolAsync(int scriptToolId, int traderId, ScriptToolConfigModel config)
        {
            var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(scriptToolId);
            var scriptToolItems = await _scriptToolItemService.GetAllScriptToolItemsAsync(scriptTool.Id);
            var scriptGroups = await _scriptGroupService.GetAllScriptGroupsAsync(traderId);

            var scriptIds = scriptToolItems.Select(x => x.ScriptId).ToArray();    
            var scripts = await _scriptService.GetScriptsByIdsAsync(scriptIds);

            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);

            var scriptList = new List<ScriptReport>();

            if (connectionResult.Success)
            {
                foreach (var script in scripts)
                {
                    var scriptItemsDict = await _scriptTraderModelService.CreateScriptItemsDictAsync(
                        script, traderId, connectionResult.Connection, connectionResult.CompanyId, config);

                    var value = _scriptTraderModelService.CreateScriptsDictItem(scriptItemsDict);
                    var scriptGroup = scriptGroups.FirstOrDefault(x => x.Id == script.ScriptGroupId);

                    var model = script.ToModel<ScriptReport>();
                    model.Value = value;
                    model.ScriptGroupAlignTypeId = scriptGroup?.ScriptAlignTypeId ?? 0;

                    scriptList.Add(model);
                }
            }

            return (scriptTool.Title, scriptList);
        }

        public virtual async Task<GridResponse<DynamicModel>> PrepareScriptReportAsync(
            IList<string> groups, int traderId, int categoryBookTypeId, string traderName, ScriptToolConfigModel config)
        {
            var scriptGroups = await _scriptGroupService.GetAllScriptGroupsAsync(traderId);
            var scripts = await _scriptService.GetAllScriptsAsync(traderId, groups);

            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);

            var scriptsDict = new List<DynamicModel>();

            if (connectionResult.Success)
            {
                foreach (var script in scripts)
                {
                    var scriptItemsDict = await _scriptTraderModelService.CreateScriptItemsDictAsync(
                        script, traderId, connectionResult.Connection, connectionResult.CompanyId, config);

                    var total = _scriptTraderModelService.CreateScriptsDictItem(scriptItemsDict);

                    scriptsDict.Add(new DynamicModel
                    {
                        Name = script.IsPercent ? $"{script.ScriptName} (%)" : script.ScriptName,
                        Group = scriptGroups.FirstOrDefault(x => x.Id == script.ScriptGroupId)?.GroupName ?? "Group",
                        Value = total,
                        Printed = script.Printed
                    });
                }

            }

            var columns = new List<GridColumn>
            {
                new GridColumn { Field = "name", Title = "Σενάριο", Filter = "text", TextAlign = "left" },
                new GridColumn { Field = "group", Title = "Ομαδοποίηση", Filter = "text", TextAlign = "left", Hidden = true },
                new GridColumn { Field = "value", Title = "Αποτελέσματα", Width = 150, Filter = "numeric", Format = "#,##0.00", TextAlign = "right", HeaderAlign = "center" }
            };

            return new GridResponse<DynamicModel>
            {
                Title = traderName,
                Data = scriptsDict.Where(x => x.Printed).OrderBy(o => o.Group).ToList(),
                Columns = columns
            };
        }

        public virtual async Task<PivotResponse> PrepareScriptPivotAsync(ScriptTraderModel model)
        {
            var list = new List<PivotDynamicModel>();
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(model.TraderId);

            if (connectionResult.Success)
            {
                var fields = await _scriptFieldService.GetAllScriptFieldsAsync(model.TraderId);
                var scriptPivotItems = (await _scriptPivotItemService.GetAllScriptPivotItemsAsync(model.Id))
                    .Where(x => x.Printed).ToList();

                foreach (var scriptPivotItem in scriptPivotItems)
                {
                    var field = fields.First(x => x.Id == scriptPivotItem.ScriptFieldId);

                    var values = ((ScriptFieldType)field.ScriptFieldTypeId) switch
                    {
                        ScriptFieldType.Table => (await _scriptTraderModelService.PrepareScriptPivotFieldAsync(
                            field, connectionResult.Connection, connectionResult.CompanyId, model.Year, model.Period, (ScriptPivotShowType)model.ShowTypeId, model.Inventory)),
                        ScriptFieldType.Query => (await _scriptTraderModelService.PrepareScriptPivotQueryAsync(
                            field, connectionResult.Connection, connectionResult.CompanyId, model.Year, model.Period, (ScriptPivotShowType)model.ShowTypeId, model.Inventory )),
                        _ => new List<(string, decimal)>()
                    };

                    foreach (var value in values)
                    {
                        list.Add(new PivotDynamicModel
                        {
                            FieldName = $"field{scriptPivotItem.Id}",
                            TitleName = field.FieldName,
                            Value = value.Item2,
                            Month = value.Item1
                        });
                    }
                }
            }

            var pivot = ScriptPivotHelper.PivotForGrid(list);

            // after the foreach that fills 'pivot'
            var idx = pivot.FindIndex(row =>
            {
                var dict = (IDictionary<string, object>)row;
                return dict.TryGetValue("period", out var period) &&
                       string.Equals(Convert.ToString(period), "Απογραφή", StringComparison.OrdinalIgnoreCase);
            });
            if (idx > 0)
            {
                var row = pivot[idx];
                pivot.RemoveAt(idx);
                pivot.Insert(0, row);
            }

            // column metadata
            var columns = new List<GridColumn>();

            var fieldNames = list.Select(x => x.FieldName).Distinct().ToList();

            foreach (var fieldName in fieldNames)
            {
                columns.Add(new GridColumn
                {
                    Field = fieldName.ToCamelCase(),
                    Title = list.First(x => x.FieldName == fieldName).TitleName,
                    //MinResizableWidth = 120,
                    Filter = "numeric",
                    Format = "#,##0.00",
                    TextAlign = "right",
                    HeaderAlign = "center"
                });
            }
            var aggregates = new List<Dictionary<string, string>>();

            foreach (var column in columns) 
            {
                var dict = new Dictionary<string, string>();
                dict.Add("field", column.Field);
                dict.Add("aggregate", "sum");

                aggregates.Add(dict);
            }

            // column metadata
            columns.Insert(0, new GridColumn { Field = "period", Title = "Περίοδος" });
            //columns.Insert(0, new GridColumn { Field = "value", Title = "Αποτελέσματα", Width = 150, Filter = "numeric", Format = "#,##0.00", TextAlign = "right", HeaderAlign = "center" });
            
            var scriptPivot = await _scriptPivotService.GetScriptPivotByIdAsync(model.Id);
            var title = $"{model.TraderName} - {scriptPivot.ScriptPivotName}";

            if (!string.IsNullOrEmpty(scriptPivot.Description))
                title = $"{title} / {scriptPivot.Description}";

            return new PivotResponse
            {
                Title = title,
                Data = pivot,
                Columns = columns,
                Aggregates = aggregates
            };
        }
    }
}