using App.Core;
using App.Core.Domain.Scripts;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Services.Scripts
{
    public partial interface IScriptItemService
    {
        IQueryable<ScriptItem> Table { get; }
        Task<ScriptItem> GetScriptItemByIdAsync(int scriptItemId);
        Task<IList<ScriptItem>> GetScriptItemsByIdsAsync(int[] scriptItemIds);
        Task<IList<ScriptItem>> GetAllScriptItemsAsync(int scriptId);
        Task<IPagedList<ScriptItemModel>> GetPagedListAsync(ScriptItemSearchModel searchModel, int parentId);
        Task DeleteScriptItemAsync(ScriptItem scriptItem);
        Task DeleteScriptItemAsync(IList<ScriptItem> criptTableNames);
        Task InsertScriptItemAsync(ScriptItem scriptItem);
        Task UpdateScriptItemAsync(ScriptItem scriptItem);
    }
    public partial class ScriptItemService : IScriptItemService
    {
        private readonly IRepository<ScriptItem> _scriptItemRepository;
        private readonly IRepository<Script> _scriptRepository;
        private readonly IRepository<ScriptGroup> _scriptGroupRepository;
        private readonly IRepository<ScriptField> _scriptFieldRepository;

        public ScriptItemService(IRepository<ScriptItem> scriptItemRepository,
            IRepository<Script> scriptRepository,
            IRepository<ScriptGroup> scriptGroupRepository,
            IRepository<ScriptField> scriptFieldRepository)
        {
            _scriptItemRepository = scriptItemRepository;
            _scriptRepository = scriptRepository;
            _scriptGroupRepository = scriptGroupRepository;
            _scriptFieldRepository = scriptFieldRepository;
        }

        public virtual IQueryable<ScriptItem> Table => _scriptItemRepository.Table;

        public virtual async Task<ScriptItem> GetScriptItemByIdAsync(int scriptItemId)
        {
            return await _scriptItemRepository.GetByIdAsync(scriptItemId);
        }

        public virtual async Task<IList<ScriptItem>> GetScriptItemsByIdsAsync(int[] scriptItemIds)
        {
            return await _scriptItemRepository.GetByIdsAsync(scriptItemIds);
        }

        public virtual async Task<IList<ScriptItem>> GetAllScriptItemsAsync(int scriptId)
        {
            var entities = await _scriptItemRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.ScriptId == scriptId).OrderBy(l => l.Order);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptItemModel>> GetPagedListAsync(ScriptItemSearchModel searchModel, int parentId)
        {
            var operations = await ScriptOperationType.Addition.ToSelectionItemListAsync();
            var scriptTypes = await ScriptType.Field.ToSelectionItemListAsync();

            var query = _scriptItemRepository.Table.AsEnumerable()
                .Where(x => x.ScriptId == parentId)
                .Select(x => 
                {
                    var model = x.ToModel<ScriptItemModel>();

                    var script = _scriptRepository.Table.FirstOrDefault(k => k.Id == x.ParentId);
                    var scriptGroupName = script == null ? "" : _scriptGroupRepository
                        .Table.FirstOrDefault(k => k.Id == script.ScriptGroupId)?.GroupName ?? "";
                    var scriptField = _scriptFieldRepository.Table.FirstOrDefault(k => k.Id == x.ScriptFieldId);
                    var fieldGroupName = scriptField == null ? "" : _scriptGroupRepository
                        .Table.FirstOrDefault(k => k.Id == scriptField.ScriptGroupId)?.GroupName ?? "";

                    model.ScriptOperationTypeName = operations.FirstOrDefault(a => a.Value == x.ScriptOperationTypeId)?.Label ?? "";
                    model.ScriptTypeName = scriptTypes.FirstOrDefault(a => a.Value == x.ScriptTypeId)?.Label ?? "";
                    model.ParentName =
                        x.ScriptTypeId == (int)ScriptType.Script ? script?.ScriptName ?? "" : scriptField?.FieldName ?? "";
                    model.ParentGroupName =
                        x.ScriptTypeId == (int)ScriptType.Script ? scriptGroupName : fieldGroupName;

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

        public virtual async Task DeleteScriptItemAsync(ScriptItem scriptItem)
        {
            await _scriptItemRepository.DeleteAsync(scriptItem);
        }

        public virtual async Task DeleteScriptItemAsync(IList<ScriptItem> scriptItems)
        {
            await _scriptItemRepository.DeleteAsync(scriptItems);
        }

        public virtual async Task InsertScriptItemAsync(ScriptItem scriptItem)
        {
            await _scriptItemRepository.InsertAsync(scriptItem);
        }

        public virtual async Task UpdateScriptItemAsync(ScriptItem scriptItem)
        {
            await _scriptItemRepository.UpdateAsync(scriptItem);
        }
    }
}