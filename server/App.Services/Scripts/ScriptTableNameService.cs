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
    public partial interface IScriptTableNameService
    {
        IQueryable<ScriptTableName> Table { get; }
        Task<ScriptTableName> GetScriptTableNameByIdAsync(int scriptTableNameId);
        Task<IList<ScriptTableName>> GetScriptTableNamesByIdsAsync(int[] scriptTableNameIds);
        Task<IList<ScriptTableName>> GetAllScriptTableNamesAsync();
        Task<IPagedList<ScriptTableNameModel>> GetPagedListAsync(ScriptTableNameSearchModel searchModel);
        Task DeleteScriptTableNameAsync(ScriptTableName scriptTableName);
        Task DeleteScriptTableNameAsync(IList<ScriptTableName> criptTableNames);
        Task InsertScriptTableNameAsync(ScriptTableName scriptTableName);
        Task UpdateScriptTableNameAsync(ScriptTableName scriptTableName);
    }
    public partial class ScriptTableNameService : IScriptTableNameService
    {
        private readonly IRepository<ScriptTableName> _scriptTableNameRepository;

        public ScriptTableNameService(IRepository<ScriptTableName> scriptTableNameRepository)
        {
            _scriptTableNameRepository = scriptTableNameRepository;
        }

        public virtual IQueryable<ScriptTableName> Table => _scriptTableNameRepository.Table;

        public virtual async Task<ScriptTableName> GetScriptTableNameByIdAsync(int scriptTableNameId)
        {
            return await _scriptTableNameRepository.GetByIdAsync(scriptTableNameId);
        }

        public virtual async Task<IList<ScriptTableName>> GetScriptTableNamesByIdsAsync(int[] scriptTableNameIds)
        {
            return await _scriptTableNameRepository.GetByIdsAsync(scriptTableNameIds);
        }

        public virtual async Task<IList<ScriptTableName>> GetAllScriptTableNamesAsync()
        {
            var entities = await _scriptTableNameRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Order);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptTableNameModel>> GetPagedListAsync(ScriptTableNameSearchModel searchModel)
        {
            var query = _scriptTableNameRepository.Table.AsEnumerable()
                .Select(x => x.ToModel<ScriptTableNameModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Name.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteScriptTableNameAsync(ScriptTableName scriptTableName)
        {
            await _scriptTableNameRepository.DeleteAsync(scriptTableName);
        }

        public virtual async Task DeleteScriptTableNameAsync(IList<ScriptTableName> scriptTableNames)
        {
            await _scriptTableNameRepository.DeleteAsync(scriptTableNames);
        }

        public virtual async Task InsertScriptTableNameAsync(ScriptTableName scriptTableName)
        {
            await _scriptTableNameRepository.InsertAsync(scriptTableName);
        }

        public virtual async Task UpdateScriptTableNameAsync(ScriptTableName scriptTableName)
        {
            await _scriptTableNameRepository.UpdateAsync(scriptTableName);
        }
    }
}