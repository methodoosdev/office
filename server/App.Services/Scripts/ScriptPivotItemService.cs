using App.Core;
using App.Core.Domain.Scripts;
using App.Core.Infrastructure;
using App.Data;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Scripts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Scripts
{
    public partial interface IScriptPivotItemService
    {
        IQueryable<ScriptPivotItem> Table { get; }
        Task<ScriptPivotItem> GetScriptPivotItemByIdAsync(int scriptPivotItemId);
        Task<IList<ScriptPivotItem>> GetScriptPivotItemsByIdsAsync(int[] scriptPivotItemIds);
        Task<IList<ScriptPivotItem>> GetAllScriptPivotItemsAsync(int scriptPivotId);
        Task<IPagedList<ScriptPivotItemModel>> GetPagedListAsync(ScriptPivotItemSearchModel searchModel, int parentId);
        Task DeleteScriptPivotItemAsync(ScriptPivotItem scriptPivotItem);
        Task DeleteScriptPivotItemAsync(IList<ScriptPivotItem> criptTableNames);
        Task InsertScriptPivotItemAsync(ScriptPivotItem scriptPivotItem);
        Task UpdateScriptPivotItemAsync(ScriptPivotItem scriptPivotItem);
    }
    public partial class ScriptPivotItemService : IScriptPivotItemService
    {
        private readonly IRepository<ScriptPivot> _scriptPivotRepository;
        private readonly IRepository<ScriptPivotItem> _scriptPivotItemRepository;
        private readonly IRepository<ScriptTableItem> _scriptTableItemRepository;
        private readonly IRepository<ScriptGroup> _scriptGroupRepository;
        private readonly IRepository<ScriptField> _scriptFieldRepository;

        public ScriptPivotItemService(
            IRepository<ScriptPivot> scriptPivotRepository,
            IRepository<ScriptPivotItem> scriptPivotItemRepository,
            IRepository<ScriptTableItem> scriptTableItemRepository,
            IRepository<ScriptGroup> scriptGroupRepository,
            IRepository<ScriptField> scriptFieldRepository)
        {
            _scriptPivotRepository = scriptPivotRepository;
            _scriptPivotItemRepository = scriptPivotItemRepository;
            _scriptTableItemRepository = scriptTableItemRepository;
            _scriptGroupRepository = scriptGroupRepository;
            _scriptFieldRepository = scriptFieldRepository;
        }

        public virtual IQueryable<ScriptPivotItem> Table => _scriptPivotItemRepository.Table;

        public virtual async Task<ScriptPivotItem> GetScriptPivotItemByIdAsync(int scriptPivotItemId)
        {
            return await _scriptPivotItemRepository.GetByIdAsync(scriptPivotItemId);
        }

        public virtual async Task<IList<ScriptPivotItem>> GetScriptPivotItemsByIdsAsync(int[] scriptPivotItemIds)
        {
            return await _scriptPivotItemRepository.GetByIdsAsync(scriptPivotItemIds);
        }

        public virtual async Task<IList<ScriptPivotItem>> GetAllScriptPivotItemsAsync(int scriptPivotId)
        {
            var entities = await _scriptPivotItemRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.ScriptPivotId == scriptPivotId).OrderBy(l => l.Order);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptPivotItemModel>> GetPagedListAsync(ScriptPivotItemSearchModel searchModel, int parentId)
        {
            //var operations = await ScriptOperationType.Addition.ToSelectionItemListAsync();
            var queries = await ScriptQueryType.Payments.ToSelectionItemListAsync();
            var fieldTypes = await ScriptFieldType.Table.ToSelectionItemListAsync();
            var scriptPivot = _scriptPivotRepository.Table.First(x => x.Id == parentId);
            var scriptGroups = _scriptGroupRepository.Table
                .Where(x => x.TraderId == scriptPivot.TraderId).ToList();

            var query = _scriptPivotItemRepository.Table.AsEnumerable()
                .Where(x => x.ScriptPivotId == parentId)
                .Select(x => 
                {
                    var scriptField = _scriptFieldRepository.Table.FirstOrDefault(k => x.ScriptFieldId == k.Id);
                    var model = x.ToModel<ScriptPivotItemModel>();

                    //model.ScriptOperationTypeName = operations.FirstOrDefault(a => a.Value == x.ScriptOperationTypeId)?.Label ?? "";
                    model.ScriptFieldName = scriptField?.FieldName ?? "";
                    model.ScriptFieldTypeName = fieldTypes.FirstOrDefault(a => a.Value == scriptField?.ScriptFieldTypeId)?.Label ?? "";
                    model.ParentGroupName = scriptGroups.FirstOrDefault(a => a.Id == scriptField?.ScriptGroupId)?.GroupName ?? "";

                    if (scriptField.ScriptFieldTypeId == (int)ScriptFieldType.Table)
                    {
                        var codes = _scriptTableItemRepository.Table
                            .Where(k => k.ScriptTableId == scriptField.ScriptTableId)
                            .Select(k => k.AccountingCode)
                            .ToArray();

                        model.ScriptDetailName = string.Join(" , ", codes);
                    }

                    if (scriptField.ScriptFieldTypeId == (int)ScriptFieldType.Query)
                        model.ScriptDetailName = queries.FirstOrDefault(a => a.Value == scriptField?.ScriptQueryTypeId)?.Label ?? "";


                    return model;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.ScriptPivotName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteScriptPivotItemAsync(ScriptPivotItem scriptPivotItem)
        {
            await _scriptPivotItemRepository.DeleteAsync(scriptPivotItem);
        }

        public virtual async Task DeleteScriptPivotItemAsync(IList<ScriptPivotItem> scriptPivotItems)
        {
            await _scriptPivotItemRepository.DeleteAsync(scriptPivotItems);
        }

        public virtual async Task InsertScriptPivotItemAsync(ScriptPivotItem scriptPivotItem)
        {
            await _scriptPivotItemRepository.InsertAsync(scriptPivotItem);
        }

        public virtual async Task UpdateScriptPivotItemAsync(ScriptPivotItem scriptPivotItem)
        {
            await _scriptPivotItemRepository.UpdateAsync(scriptPivotItem);
        }
    }
}