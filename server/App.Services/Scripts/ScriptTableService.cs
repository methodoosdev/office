using App.Core;
using App.Core.Domain.Scripts;
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
    public partial interface IScriptTableService
    {
        IQueryable<ScriptTable> Table { get; }
        Task<ScriptTable> GetScriptTableByIdAsync(int scriptTableId);
        Task<IList<ScriptTable>> GetScriptTablesByIdsAsync(int[] scriptTableIds);
        Task<IList<ScriptTable>> GetAllScriptTablesAsync(int traderId);
        Task<IPagedList<ScriptTableModel>> GetPagedListAsync(ScriptTableSearchModel searchModel, int parentId);
        Task DeleteScriptTableAsync(Expression<Func<ScriptTable, bool>> predicate);
        Task DeleteScriptTableAsync(ScriptTable scriptTable);
        Task DeleteScriptTableAsync(IList<ScriptTable> criptTableNames);
        Task InsertScriptTableAsync(ScriptTable scriptTable);
        Task UpdateScriptTableAsync(ScriptTable scriptTable);
    }
    public partial class ScriptTableService : IScriptTableService
    {
        private readonly IRepository<ScriptTable> _scriptTableRepository;
        private readonly IRepository<ScriptGroup> _scriptGroupRepository;

        public ScriptTableService(
            IRepository<ScriptTable> scriptTableRepository,
            IRepository<ScriptGroup> scriptGroupRepository
            )
        {
            _scriptTableRepository = scriptTableRepository;
            _scriptGroupRepository = scriptGroupRepository;
        }

        public virtual IQueryable<ScriptTable> Table => _scriptTableRepository.Table;

        public virtual async Task<ScriptTable> GetScriptTableByIdAsync(int scriptTableId)
        {
            return await _scriptTableRepository.GetByIdAsync(scriptTableId);
        }

        public virtual async Task<IList<ScriptTable>> GetScriptTablesByIdsAsync(int[] scriptTableIds)
        {
            return await _scriptTableRepository.GetByIdsAsync(scriptTableIds);
        }

        public virtual async Task<IList<ScriptTable>> GetAllScriptTablesAsync(int traderId)
        {
            var entities = await _scriptTableRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId).OrderBy(l => l.Order);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptTableModel>> GetPagedListAsync(ScriptTableSearchModel searchModel, int parentId)
        {
            var scriptGroups = _scriptGroupRepository.Table.Where(x => x.TraderId == parentId).ToList();

            var query = _scriptTableRepository.Table.AsEnumerable()
                .Where(c => c.TraderId == parentId)
                .Select(x => 
                {
                    var scriptGroup = scriptGroups.FirstOrDefault(k => k.Id == x.ScriptGroupId);

                    var model = x.ToModel<ScriptTableModel>();
                    model.ScriptGroupName = scriptGroup?.GroupName ?? "";
                    //model.HasChildren = _scriptTableItemRepository.Table.Any(k => k.ScriptTableId == x.Id);

                    return model;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.TableName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteScriptTableAsync(Expression<Func<ScriptTable, bool>> predicate)
        {
            await _scriptTableRepository.DeleteAsync(predicate);
        }

        public virtual async Task DeleteScriptTableAsync(ScriptTable scriptTable)
        {
            await _scriptTableRepository.DeleteAsync(scriptTable);
        }

        public virtual async Task DeleteScriptTableAsync(IList<ScriptTable> scriptTables)
        {
            await _scriptTableRepository.DeleteAsync(scriptTables);
        }

        public virtual async Task InsertScriptTableAsync(ScriptTable scriptTable)
        {
            await _scriptTableRepository.InsertAsync(scriptTable);
        }

        public virtual async Task UpdateScriptTableAsync(ScriptTable scriptTable)
        {
            await _scriptTableRepository.UpdateAsync(scriptTable);
        }
    }
}