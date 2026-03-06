using App.Core;
using App.Core.Domain.Customers;
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
    public partial interface IScriptService
    {
        IQueryable<Script> Table { get; }
        Task<Script> GetScriptByIdAsync(int scriptId);
        Task<IList<Script>> GetScriptsByIdsAsync(int[] scriptIds);
        Task<Script> GetScriptByReplacementAsync(int traderId, int id, string replacement);
        Task<Dictionary<string, Script>> GetScriptIdsByTokensAsync(int traderId, IList<string> tokens);
        Task<IList<Script>> GetAllScriptsAsync(int traderId);
        Task<IList<Script>> GetAllScriptsAsync(int traderId, IList<string> groups);
        Task<IPagedList<ScriptModel>> GetPagedListAsync(ScriptSearchModel searchModel, int traderId);
        Task DeleteScriptAsync(Expression<Func<Script, bool>> predicate);
        Task DeleteScriptAsync(Script script);
        Task DeleteScriptAsync(IList<Script> criptTableNames);
        Task InsertScriptAsync(Script script);
        Task UpdateScriptAsync(Script script);
    }
    public partial class ScriptService : IScriptService
    {
        private readonly IRepository<Script> _scriptRepository;
        private readonly IRepository<ScriptGroup> _scriptGroupRepository;

        public ScriptService(
            IRepository<Script> scriptRepository, 
            IRepository<ScriptGroup> scriptGroupRepository)
        {
            _scriptRepository = scriptRepository;
            _scriptGroupRepository = scriptGroupRepository;
        }

        public virtual IQueryable<Script> Table => _scriptRepository.Table;

        public virtual async Task<Script> GetScriptByIdAsync(int scriptId)
        {
            return await _scriptRepository.GetByIdAsync(scriptId);
        }

        public virtual async Task<IList<Script>> GetScriptsByIdsAsync(int[] scriptIds)
        {
            return await _scriptRepository.GetByIdsAsync(scriptIds);
        }

        public virtual async Task<Script> GetScriptByReplacementAsync(int traderId, int id, string replacement)
        {
            if (string.IsNullOrWhiteSpace(replacement))
                return null;

            var query = from c in _scriptRepository.Table
                        orderby c.Id
                        where c.Replacement.Trim() == replacement.Trim() && c.TraderId == traderId &&  c.Id != id
                        select c;
            var customer = await query.FirstOrDefaultAsync();

            return customer;
        }

        public virtual async Task<Dictionary<string, Script>> GetScriptIdsByTokensAsync(int traderId, IList<string> tokens)
        {
            var scripts = await GetAllScriptsAsync(traderId);

            // Build a lookup from Replacement -> Script, skipping dup keys (keep first)
            var scriptLookup = new Dictionary<string, Script>(StringComparer.Ordinal);
            foreach (var s in scripts)
            {
                var key = s.Replacement?.Trim();
                if (string.IsNullOrEmpty(key)) continue;

                // TryAdd avoids "same key" crashes if duplicates exist
                scriptLookup.TryAdd(key, s);
            }

            var tokenSet = new HashSet<string>(tokens, StringComparer.Ordinal);
            var dict = new Dictionary<string, Script>(StringComparer.Ordinal);

            foreach (var tok in tokenSet)
            {
                var t = tok.Trim();

                // exact match first (e.g., "#some" -> script "#some")
                if (scriptLookup.TryGetValue(t, out var exact))
                {
                    dict[t] = exact;
                    continue;
                }

                // if token ends with '-', try its base (e.g., "#any-" -> lookup "#any")
                if (t.EndsWith("-", StringComparison.Ordinal))
                {
                    var baseKey = t[..^1];
                    if (scriptLookup.TryGetValue(baseKey, out var fromBase))
                        dict[t] = fromBase;
                }
            }

            return dict;
        }

        public virtual async Task<IList<Script>> GetAllScriptsAsync(int traderId)
        {
            var entities = await _scriptRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId).OrderBy(l => l.Order);

                return query;
            });

            return entities;
        }

        public virtual async Task<IList<Script>> GetAllScriptsAsync(int traderId, IList<string> groups)
        {
            var scriptGroupIds = _scriptGroupRepository.Table
                .Where(x => x.TraderId == traderId && groups.Contains(x.GroupName))
                .Select(x => x.Id).ToList();

            var entities = await _scriptRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId && scriptGroupIds.Contains(x.ScriptGroupId));

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptModel>> GetPagedListAsync(ScriptSearchModel searchModel, int traderId)
        {
            var scriptGroups = _scriptGroupRepository.Table.Where(x => x.TraderId == traderId).ToList();

            var query = _scriptRepository.Table.AsEnumerable()
                .Where(x => x.TraderId == traderId)
                .Select(x =>
                {
                    var scriptGroup = scriptGroups.FirstOrDefault(k => k.Id == x.ScriptGroupId);

                    var model = x.ToModel<ScriptModel>();
                    model.ScriptGroupName = scriptGroup?.GroupName ?? "";

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

        public virtual async Task DeleteScriptAsync(Expression<Func<Script, bool>> predicate)
        {
            await _scriptRepository.DeleteAsync(predicate);
        }

        public virtual async Task DeleteScriptAsync(Script script)
        {
            await _scriptRepository.DeleteAsync(script);
        }

        public virtual async Task DeleteScriptAsync(IList<Script> scripts)
        {
            await _scriptRepository.DeleteAsync(scripts);
        }

        public virtual async Task InsertScriptAsync(Script script)
        {
            await _scriptRepository.InsertAsync(script);
        }

        public virtual async Task UpdateScriptAsync(Script script)
        {
            await _scriptRepository.UpdateAsync(script);
        }
    }
}