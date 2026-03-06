using App.Core;
using App.Core.Domain.Scripts;
using App.Core.Infrastructure;
using App.Data;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Logging;
using App.Models.Scripts;
using App.Services.Helpers;
using MathNet.Numerics.Statistics.Mcmc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace App.Services.Scripts
{
    public partial interface IScriptToolService
    {
        IQueryable<ScriptTool> Table { get; }
        Task<ScriptTool> GetScriptToolByIdAsync(int scriptToolId);
        Task<IList<ScriptTool>> GetScriptToolsByIdsAsync(int[] scriptToolIds);
        Task<IList<ScriptTool>> GetAllScriptToolsAsync(int traderId);
        Task<IPagedList<ScriptToolModel>> GetPagedListAsync(ScriptToolSearchModel searchModel, int traderId);
        Task DeleteScriptToolAsync(Expression<Func<ScriptTool, bool>> predicate);
        Task DeleteScriptToolAsync(ScriptTool scriptTool);
        Task DeleteScriptToolAsync(IList<ScriptTool> criptTableNames);
        Task InsertScriptToolAsync(ScriptTool scriptTool);
        Task UpdateScriptToolAsync(ScriptTool scriptTool);
    }
    public partial class ScriptToolService : IScriptToolService
    {
        protected readonly IDateTimeHelper _dateTimeHelper;
        protected readonly IRepository<ScriptTool> _scriptToolRepository;

        public ScriptToolService(
            IDateTimeHelper dateTimeHelper,
            IRepository<ScriptTool> scriptToolRepository)
        {
            _dateTimeHelper = dateTimeHelper;
            _scriptToolRepository = scriptToolRepository;
        }

        public virtual IQueryable<ScriptTool> Table => _scriptToolRepository.Table;

        public virtual async Task<ScriptTool> GetScriptToolByIdAsync(int scriptToolId)
        {
            return await _scriptToolRepository.GetByIdAsync(scriptToolId);
        }

        public virtual async Task<IList<ScriptTool>> GetScriptToolsByIdsAsync(int[] scriptToolIds)
        {
            return await _scriptToolRepository.GetByIdsAsync(scriptToolIds);
        }

        public virtual async Task<IList<ScriptTool>> GetAllScriptToolsAsync(int traderId)
        {
            var entities = await _scriptToolRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId).OrderBy(l => l.Order);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptToolModel>> GetPagedListAsync(ScriptToolSearchModel searchModel, int traderId)
        {
            var query = _scriptToolRepository.Table.AsEnumerable()
                .Where(x => x.TraderId == traderId)
                .Select(x => 
                {
                    var model = x.ToModel<ScriptToolModel>();

                    if (x.CreatedOnUtc.HasValue)
                        model.CreatedOn = _dateTimeHelper.ConvertToUserTimeAsync(x.CreatedOnUtc.Value, DateTimeKind.Utc).Result;

                    return model;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Title.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteScriptToolAsync(Expression<Func<ScriptTool, bool>> predicate)
        {
            await _scriptToolRepository.DeleteAsync(predicate);
        }

        public virtual async Task DeleteScriptToolAsync(ScriptTool scriptTool)
        {
            await _scriptToolRepository.DeleteAsync(scriptTool);
        }

        public virtual async Task DeleteScriptToolAsync(IList<ScriptTool> scriptTools)
        {
            await _scriptToolRepository.DeleteAsync(scriptTools);
        }

        public virtual async Task InsertScriptToolAsync(ScriptTool scriptTool)
        {
            await _scriptToolRepository.InsertAsync(scriptTool);
        }

        public virtual async Task UpdateScriptToolAsync(ScriptTool scriptTool)
        {
            await _scriptToolRepository.UpdateAsync(scriptTool);
        }
    }
}