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
    public partial interface IScriptTableItemService
    {
        IQueryable<ScriptTableItem> Table { get; }
        Task<ScriptTableItem> GetScriptTableItemByIdAsync(int ScriptTableItemId);
        Task<IList<ScriptTableItem>> GetScriptTableItemsByIdsAsync(int[] ScriptTableItemIds);
        Task<IList<ScriptTableItem>> GetAllScriptTableItemsAsync(int scriptTableId);
        Task<IPagedList<ScriptTableItemModel>> GetPagedListAsync(ScriptTableItemSearchModel searchModel, int parentId);
        Task DeleteScriptTableItemAsync(ScriptTableItem ScriptTableItem);
        Task DeleteScriptTableItemAsync(IList<ScriptTableItem> criptTableNames);
        Task InsertScriptTableItemAsync(ScriptTableItem ScriptTableItem);
        Task UpdateScriptTableItemAsync(ScriptTableItem ScriptTableItem);
    }
    public partial class ScriptTableItemService : IScriptTableItemService
    {
        private readonly IRepository<ScriptTable> _scriptTableRepository;
        private readonly IRepository<ScriptTableItem> _scriptTableItemRepository;

        public ScriptTableItemService(
            IRepository<ScriptTable> scriptTableRepository,
            IRepository<ScriptTableItem> scriptTableItemRepository)
        {
            _scriptTableRepository = scriptTableRepository;
            _scriptTableItemRepository = scriptTableItemRepository;
        }

        public virtual IQueryable<ScriptTableItem> Table => _scriptTableItemRepository.Table;

        public virtual async Task<ScriptTableItem> GetScriptTableItemByIdAsync(int scriptTableItemId)
        {
            return await _scriptTableItemRepository.GetByIdAsync(scriptTableItemId);
        }

        public virtual async Task<IList<ScriptTableItem>> GetScriptTableItemsByIdsAsync(int[] scriptTableItemIds)
        {
            return await _scriptTableItemRepository.GetByIdsAsync(scriptTableItemIds);
        }

        public virtual async Task<IList<ScriptTableItem>> GetAllScriptTableItemsAsync(int scriptTableId)
        {
            var entities = await _scriptTableItemRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.ScriptTableId == scriptTableId).OrderBy(l => l.AccountingCode);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptTableItemModel>> GetPagedListAsync(ScriptTableItemSearchModel searchModel, int parentId)
        {
            var behaviors = await ScriptBehaviorType.Included.ToSelectionItemListAsync();
            var query = _scriptTableItemRepository.Table.AsEnumerable()
                .Where(c => c.ScriptTableId == parentId)
                .Select(x => 
                {
                    var model = x.ToModel<ScriptTableItemModel>();
                    model.ScriptBehaviorTypeName = behaviors.FirstOrDefault(a => a.Value == x.ScriptBehaviorTypeId)?.Label ?? "";

                    return model;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.AccountingCode.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteScriptTableItemAsync(ScriptTableItem scriptTableItem)
        {
            await _scriptTableItemRepository.DeleteAsync(scriptTableItem);
        }

        public virtual async Task DeleteScriptTableItemAsync(IList<ScriptTableItem> scriptTableItems)
        {
            await _scriptTableItemRepository.DeleteAsync(scriptTableItems);
        }

        public virtual async Task InsertScriptTableItemAsync(ScriptTableItem scriptTableItem)
        {
            await _scriptTableItemRepository.InsertAsync(scriptTableItem);
        }

        public virtual async Task UpdateScriptTableItemAsync(ScriptTableItem scriptTableItem)
        {
            await _scriptTableItemRepository.UpdateAsync(scriptTableItem);
        }
    }
}