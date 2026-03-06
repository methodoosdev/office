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
    public partial interface IScriptPivotService
    {
        IQueryable<ScriptPivot> Table { get; }
        Task<ScriptPivot> GetScriptPivotByIdAsync(int scriptPivotId);
        Task<IList<ScriptPivot>> GetScriptPivotsByIdsAsync(int[] scriptPivotIds);
        Task<IList<ScriptPivot>> GetAllScriptPivotsAsync(int traderId);
        Task<IPagedList<ScriptPivotModel>> GetPagedListAsync(ScriptPivotSearchModel searchModel, int traderId);
        Task DeleteScriptPivotAsync(Expression<Func<ScriptPivot, bool>> predicate);
        Task DeleteScriptPivotAsync(ScriptPivot scriptPivot);
        Task DeleteScriptPivotAsync(IList<ScriptPivot> criptTableNames);
        Task InsertScriptPivotAsync(ScriptPivot scriptPivot);
        Task UpdateScriptPivotAsync(ScriptPivot scriptPivot);
    }
    public partial class ScriptPivotService : IScriptPivotService
    {
        private readonly IRepository<ScriptPivot> _scriptPivotRepository;
        private readonly IRepository<ScriptGroup> _scriptGroupRepository;

        public ScriptPivotService(
            IRepository<ScriptPivot> scriptPivotRepository, 
            IRepository<ScriptGroup> scriptGroupRepository)
        {
            _scriptPivotRepository = scriptPivotRepository;
            _scriptGroupRepository = scriptGroupRepository;
        }

        public virtual IQueryable<ScriptPivot> Table => _scriptPivotRepository.Table;

        public virtual async Task<ScriptPivot> GetScriptPivotByIdAsync(int scriptPivotId)
        {
            return await _scriptPivotRepository.GetByIdAsync(scriptPivotId);
        }

        public virtual async Task<IList<ScriptPivot>> GetScriptPivotsByIdsAsync(int[] scriptPivotIds)
        {
            return await _scriptPivotRepository.GetByIdsAsync(scriptPivotIds);
        }

        public virtual async Task<IList<ScriptPivot>> GetAllScriptPivotsAsync(int traderId)
        {
            var entities = await _scriptPivotRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId).OrderBy(l => l.Order);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptPivotModel>> GetPagedListAsync(ScriptPivotSearchModel searchModel, int traderId)
        {
            var scriptGroups = _scriptGroupRepository.Table.Where(x => x.TraderId == traderId).ToList();

            var query = _scriptPivotRepository.Table.AsEnumerable()
                .Where(x => x.TraderId == traderId)
                .Select(x =>
                {
                    var scriptGroup = scriptGroups.FirstOrDefault(k => k.Id == x.ScriptGroupId);

                    var model = x.ToModel<ScriptPivotModel>();
                    model.ScriptGroupName = scriptGroup?.GroupName ?? "";

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

        public virtual async Task DeleteScriptPivotAsync(Expression<Func<ScriptPivot, bool>> predicate)
        {
            await _scriptPivotRepository.DeleteAsync(predicate);
        }

        public virtual async Task DeleteScriptPivotAsync(ScriptPivot scriptPivot)
        {
            await _scriptPivotRepository.DeleteAsync(scriptPivot);
        }

        public virtual async Task DeleteScriptPivotAsync(IList<ScriptPivot> scriptPivots)
        {
            await _scriptPivotRepository.DeleteAsync(scriptPivots);
        }

        public virtual async Task InsertScriptPivotAsync(ScriptPivot scriptPivot)
        {
            await _scriptPivotRepository.InsertAsync(scriptPivot);
        }

        public virtual async Task UpdateScriptPivotAsync(ScriptPivot scriptPivot)
        {
            await _scriptPivotRepository.UpdateAsync(scriptPivot);
        }
    }
}