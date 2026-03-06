using App.Core;
using App.Core.Domain.Scripts;
using App.Core.Infrastructure;
using App.Data;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Scripts;
using NPOI.POIFS.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Services.Scripts
{
    public partial interface IScriptFieldService
    {
        IQueryable<ScriptField> Table { get; }
        Task<ScriptField> GetScriptFieldByIdAsync(int scriptFieldId);
        Task<IList<ScriptField>> GetScriptFieldsByIdsAsync(int[] scriptFieldIds);
        Task<IList<ScriptField>> GetAllScriptFieldsAsync( int traderId);
        Task<IPagedList<ScriptFieldModel>> GetPagedListAsync(ScriptFieldSearchModel searchModel, int traderId);
        Task DeleteScriptFieldAsync(Expression<Func<ScriptField, bool>> predicate);
        Task DeleteScriptFieldAsync(ScriptField scriptField);
        Task DeleteScriptFieldAsync(IList<ScriptField> criptTableNames);
        Task InsertScriptFieldAsync(ScriptField scriptField);
        Task UpdateScriptFieldAsync(ScriptField scriptField);
    }
    public partial class ScriptFieldService : IScriptFieldService
    {
        private readonly IRepository<ScriptField> _scriptFieldRepository;
        private readonly IRepository<ScriptTableItem> _scriptTableItemRepository;
        private readonly IRepository<ScriptGroup> _scriptGroupRepository;

        public ScriptFieldService(
            IRepository<ScriptField> scriptFieldRepository,            
            IRepository<ScriptTableItem> scriptTableItemRepository,
            IRepository<ScriptGroup> scriptGroupRepository)
        {
            _scriptFieldRepository = scriptFieldRepository;
            _scriptTableItemRepository = scriptTableItemRepository;
            _scriptGroupRepository = scriptGroupRepository;
        }

        public virtual IQueryable<ScriptField> Table => _scriptFieldRepository.Table;

        public virtual async Task<ScriptField> GetScriptFieldByIdAsync(int scriptFieldId)
        {
            return await _scriptFieldRepository.GetByIdAsync(scriptFieldId);
        }

        public virtual async Task<IList<ScriptField>> GetScriptFieldsByIdsAsync(int[] scriptFieldIds)
        {
            return await _scriptFieldRepository.GetByIdsAsync(scriptFieldIds);
        }

        public virtual async Task<IList<ScriptField>> GetAllScriptFieldsAsync(int traderId)
        {
            var entities = await _scriptFieldRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId).OrderBy(l => l.Order);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<ScriptFieldModel>> GetPagedListAsync(ScriptFieldSearchModel searchModel, int traderId)
        {
            var aggregates = await ScriptAggregateType.Sum.ToSelectionItemListAsync();
            var functions = await ScriptFunctionType.TaxesFee.ToSelectionItemListAsync();
            var fieldTypes = await ScriptFieldType.Table.ToSelectionItemListAsync();
            var queries = await ScriptQueryType.Payments.ToSelectionItemListAsync();

            var scriptGroups = _scriptGroupRepository.Table.Where(x => x.TraderId == traderId).ToList();

            var query = _scriptFieldRepository.Table.AsEnumerable()
                .Where(l => l.TraderId == traderId)
                .Select(x =>
                {
                    var scriptGroup = scriptGroups.FirstOrDefault(k => k.Id == x.ScriptGroupId);

                    var model = x.ToModel<ScriptFieldModel>();

                    model.ScriptAggregateTypeName = aggregates.FirstOrDefault(a => a.Value == x.ScriptAggregateTypeId)?.Label ?? "";
                    model.ScriptFieldTypeName = fieldTypes.FirstOrDefault(a => a.Value == x.ScriptFieldTypeId)?.Label ?? "";

                    model.ScriptGroupName = scriptGroup?.GroupName ?? "";

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Table)
                    {
                        var codes = _scriptTableItemRepository.Table
                            .Where(k => k.ScriptTableId == x.ScriptTableId)
                            .Select(k => k.AccountingCode)
                            .ToArray();

                        model.ScriptDetailName = string.Join(" , ", codes);
                    }

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Query)
                        model.ScriptDetailName = queries.FirstOrDefault(a => a.Value == x.ScriptQueryTypeId)?.Label ?? "";

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Function)
                        model.ScriptDetailName = functions.FirstOrDefault(a => a.Value == x.ScriptFunctionTypeId)?.Label ?? "";

                    if (x.ScriptFieldTypeId == (int)ScriptFieldType.Fixed)
                        model.ScriptDetailName = x.FixedValue.ToString("n2");



                    return model;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.FieldName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteScriptFieldAsync(Expression<Func<ScriptField, bool>> predicate)
        {
            await _scriptFieldRepository.DeleteAsync(predicate);
        }

        public virtual async Task DeleteScriptFieldAsync(ScriptField scriptField)
        {
            await _scriptFieldRepository.DeleteAsync(scriptField);
        }

        public virtual async Task DeleteScriptFieldAsync(IList<ScriptField> scriptFields)
        {
            await _scriptFieldRepository.DeleteAsync(scriptFields);
        }

        public virtual async Task InsertScriptFieldAsync(ScriptField scriptField)
        {
            await _scriptFieldRepository.InsertAsync(scriptField);
        }

        public virtual async Task UpdateScriptFieldAsync(ScriptField scriptField)
        {
            await _scriptFieldRepository.UpdateAsync(scriptField);
        }
    }
}