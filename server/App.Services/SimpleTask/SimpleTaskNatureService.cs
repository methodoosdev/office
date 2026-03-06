using App.Core.Domain.SimpleTask;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.SimpleTask
{
    public partial interface ISimpleTaskNatureService
    {
        IQueryable<SimpleTaskNature> Table { get; }
        Task<SimpleTaskNature> GetSimpleTaskNatureByIdAsync(int simpleTaskNatureId);
        Task<IList<SimpleTaskNature>> GetSimpleTaskNaturesByIdsAsync(int[] simpleTaskNatureIds);
        Task<IList<SimpleTaskNature>> GetAllSimpleTaskNaturesAsync();
        Task DeleteSimpleTaskNatureAsync(SimpleTaskNature simpleTaskNature);
        Task DeleteSimpleTaskNatureAsync(IList<SimpleTaskNature> simpleTaskNatures);
        Task InsertSimpleTaskNatureAsync(SimpleTaskNature simpleTaskNature);
        Task UpdateSimpleTaskNatureAsync(SimpleTaskNature simpleTaskNature);
    }
    public partial class SimpleTaskNatureService : ISimpleTaskNatureService
    {
        private readonly IRepository<SimpleTaskNature> _simpleTaskNatureRepository;

        public SimpleTaskNatureService(
            IRepository<SimpleTaskNature> simpleTaskNatureRepository)
        {
            _simpleTaskNatureRepository = simpleTaskNatureRepository;
        }

        public virtual IQueryable<SimpleTaskNature> Table => _simpleTaskNatureRepository.Table;

        public virtual async Task<SimpleTaskNature> GetSimpleTaskNatureByIdAsync(int simpleTaskNatureId)
        {
            return await _simpleTaskNatureRepository.GetByIdAsync(simpleTaskNatureId);
        }

        public virtual async Task<IList<SimpleTaskNature>> GetSimpleTaskNaturesByIdsAsync(int[] simpleTaskNatureIds)
        {
            return await _simpleTaskNatureRepository.GetByIdsAsync(simpleTaskNatureIds);
        }

        public virtual async Task<IList<SimpleTaskNature>> GetAllSimpleTaskNaturesAsync()
        {
            var entities = await _simpleTaskNatureRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteSimpleTaskNatureAsync(SimpleTaskNature simpleTaskNature)
        {
            await _simpleTaskNatureRepository.DeleteAsync(simpleTaskNature);
        }

        public virtual async Task DeleteSimpleTaskNatureAsync(IList<SimpleTaskNature> simpleTaskNatures)
        {
            await _simpleTaskNatureRepository.DeleteAsync(simpleTaskNatures);
        }

        public virtual async Task InsertSimpleTaskNatureAsync(SimpleTaskNature simpleTaskNature)
        {
            await _simpleTaskNatureRepository.InsertAsync(simpleTaskNature);
        }

        public virtual async Task UpdateSimpleTaskNatureAsync(SimpleTaskNature simpleTaskNature)
        {
            await _simpleTaskNatureRepository.UpdateAsync(simpleTaskNature);
        }
    }
}