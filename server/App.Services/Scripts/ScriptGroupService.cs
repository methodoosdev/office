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
    public partial interface IScriptGroupService
    {
        IQueryable<ScriptGroup> Table { get; }
        Task<ScriptGroup> GetScriptGroupByIdAsync(int scriptGroupId);
        Task<IList<ScriptGroup>> GetScriptGroupsByIdsAsync(int[] scriptGroupIds);
        Task<IList<ScriptGroup>> GetAllScriptGroupsAsync(int traderId);
        Task<IPagedList<ScriptGroupModel>> GetPagedListAsync(ScriptGroupSearchModel searchModel, int parentId);
        Task DeleteScriptGroupAsync(Expression<Func<ScriptGroup, bool>> predicate);
        Task DeleteScriptGroupAsync(ScriptGroup scriptGroup);
        Task DeleteScriptGroupAsync(IList<ScriptGroup> criptTableNames);
        Task InsertScriptGroupAsync(ScriptGroup scriptGroup);
        Task UpdateScriptGroupAsync(ScriptGroup scriptGroup);
    }
    public partial class ScriptGroupService : IScriptGroupService
    {
        private readonly IRepository<ScriptGroup> _scriptGroupRepository;

        public ScriptGroupService(IRepository<ScriptGroup> scriptGroupRepository)
        {
            _scriptGroupRepository = scriptGroupRepository;
        }

        public virtual IQueryable<ScriptGroup> Table => _scriptGroupRepository.Table;

        public virtual async Task<ScriptGroup> GetScriptGroupByIdAsync(int scriptGroupId)
        {
            return await _scriptGroupRepository.GetByIdAsync(scriptGroupId);
        }

        public virtual async Task<IList<ScriptGroup>> GetScriptGroupsByIdsAsync(int[] scriptGroupIds)
        {
            return await _scriptGroupRepository.GetByIdsAsync(scriptGroupIds);
        }

        public virtual async Task<IList<ScriptGroup>> GetAllScriptGroupsAsync(int traderId)
        {
            var entities = await _scriptGroupRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId).OrderBy(l => l.Order);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptGroupModel>> GetPagedListAsync(ScriptGroupSearchModel searchModel, int parentId)
        {
            var aligns = await ScriptAlignType.Left.ToSelectionItemListAsync();

            var query = _scriptGroupRepository.Table.AsEnumerable()
                .Where(c => c.TraderId == parentId)
                .Select(x => 
                {
                    var model = x.ToModel<ScriptGroupModel>();
                    //model.ScriptAlignTypeName = aligns.FirstOrDefault(k => k.Value == x.ScriptAlignTypeId)?.Label ?? "";

                    return model;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.GroupName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteScriptGroupAsync(Expression<Func<ScriptGroup, bool>> predicate)
        {
            await _scriptGroupRepository.DeleteAsync(predicate);
        }

        public virtual async Task DeleteScriptGroupAsync(ScriptGroup scriptGroup)
        {
            await _scriptGroupRepository.DeleteAsync(scriptGroup);
        }

        public virtual async Task DeleteScriptGroupAsync(IList<ScriptGroup> scriptGroups)
        {
            await _scriptGroupRepository.DeleteAsync(scriptGroups);
        }

        public virtual async Task InsertScriptGroupAsync(ScriptGroup scriptGroup)
        {
            await _scriptGroupRepository.InsertAsync(scriptGroup);
        }

        public virtual async Task UpdateScriptGroupAsync(ScriptGroup scriptGroup)
        {
            await _scriptGroupRepository.UpdateAsync(scriptGroup);
        }
    }
}