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
    public partial interface IScriptToolItemService
    {
        IQueryable<ScriptToolItem> Table { get; }
        Task<ScriptToolItem> GetScriptToolItemByIdAsync(int scriptToolItemId);
        Task<IList<ScriptToolItem>> GetScriptToolItemsByIdsAsync(int[] scriptToolItemIds);
        Task<IList<ScriptToolItem>> GetAllScriptToolItemsAsync(int scriptToolId);
        Task<IPagedList<ScriptToolItemModel>> GetPagedListAsync(ScriptToolItemSearchModel searchModel, int parentId, int traderId);
        Task DeleteScriptToolItemAsync(ScriptToolItem scriptToolItem);
        Task DeleteScriptToolItemAsync(IList<ScriptToolItem> criptTableNames);
        Task InsertScriptToolItemAsync(ScriptToolItem scriptToolItem);
        Task UpdateScriptToolItemAsync(ScriptToolItem scriptToolItem);
    }
    public partial class ScriptToolItemService : IScriptToolItemService
    {
        private readonly IRepository<ScriptGroup> _scriptGroupRepository;
        private readonly IRepository<ScriptToolItem> _scriptToolItemRepository;
        private readonly IRepository<Script> _scriptRepository;

        public ScriptToolItemService(
            IRepository<ScriptGroup> scriptGroupRepository,
            IRepository<ScriptToolItem> scriptToolItemRepository,
            IRepository<Script> scriptRepository)
        {
            _scriptGroupRepository = scriptGroupRepository;
            _scriptToolItemRepository = scriptToolItemRepository;
            _scriptRepository = scriptRepository;
        }

        public virtual IQueryable<ScriptToolItem> Table => _scriptToolItemRepository.Table;

        public virtual async Task<ScriptToolItem> GetScriptToolItemByIdAsync(int scriptToolItemId)
        {
            return await _scriptToolItemRepository.GetByIdAsync(scriptToolItemId);
        }

        public virtual async Task<IList<ScriptToolItem>> GetScriptToolItemsByIdsAsync(int[] scriptToolItemIds)
        {
            return await _scriptToolItemRepository.GetByIdsAsync(scriptToolItemIds);
        }

        public virtual async Task<IList<ScriptToolItem>> GetAllScriptToolItemsAsync(int scriptToolId)
        {
            var entities = await _scriptToolItemRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.ScriptToolId == scriptToolId).OrderBy(l => l.Id);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptToolItemModel>> GetPagedListAsync(ScriptToolItemSearchModel searchModel, int parentId, int traderId)
        {
            var scriptGroups = await _scriptGroupRepository.Table.Where(x => x.TraderId == traderId).ToListAsync();
            var scripts = await _scriptRepository.Table.Where(x => x.TraderId == traderId).ToListAsync();

            var query = _scriptToolItemRepository.Table.AsEnumerable()
                .Where(x => x.ScriptToolId == parentId)
                .Select(x =>
                {
                    var model = x.ToModel<ScriptToolItemModel>();

                    var script = scripts.FirstOrDefault(k => k.Id == x.ScriptId);
                    if (script != null)
                    {
                        model.Order = script.Order;
                        model.ScriptName = script.ScriptName;
                        model.ScriptGroupName = scriptGroups.First(k => k.Id == script.ScriptGroupId).GroupName;
                    }

                    return model;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.ScriptName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteScriptToolItemAsync(ScriptToolItem scriptToolItem)
        {
            await _scriptToolItemRepository.DeleteAsync(scriptToolItem);
        }

        public virtual async Task DeleteScriptToolItemAsync(IList<ScriptToolItem> scriptToolItems)
        {
            await _scriptToolItemRepository.DeleteAsync(scriptToolItems);
        }

        public virtual async Task InsertScriptToolItemAsync(ScriptToolItem scriptToolItem)
        {
            await _scriptToolItemRepository.InsertAsync(scriptToolItem);
        }

        public virtual async Task UpdateScriptToolItemAsync(ScriptToolItem scriptToolItem)
        {
            await _scriptToolItemRepository.UpdateAsync(scriptToolItem);
        }
    }
}