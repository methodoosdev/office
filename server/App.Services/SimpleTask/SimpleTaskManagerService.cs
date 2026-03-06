using App.Core.Domain.SimpleTask;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.SimpleTask
{
    public partial interface ISimpleTaskManagerService
    {
        IQueryable<SimpleTaskManager> Table { get; }
        Task<SimpleTaskManager> GetSimpleTaskManagerByIdAsync(int simpleTaskManagerId);
        Task<IList<SimpleTaskManager>> GetSimpleTaskManagersByIdsAsync(int[] simpleTaskManagerIds);
        Task<IList<SimpleTaskManager>> GetAllSimpleTaskManagersAsync();
        Task DeleteSimpleTaskManagerAsync(SimpleTaskManager simpleTaskManager);
        Task DeleteSimpleTaskManagerAsync(IList<SimpleTaskManager> simpleTaskManagers);
        Task InsertSimpleTaskManagerAsync(SimpleTaskManager simpleTaskManager);
        Task UpdateSimpleTaskManagerAsync(SimpleTaskManager simpleTaskManager);
    }
    public partial class SimpleTaskManagerService : ISimpleTaskManagerService
    {
        private readonly IRepository<SimpleTaskManager> _simpleTaskManagerRepository;

        public SimpleTaskManagerService(
            IRepository<SimpleTaskManager> simpleTaskManagerRepository)
        {
            _simpleTaskManagerRepository = simpleTaskManagerRepository;
        }

        public virtual IQueryable<SimpleTaskManager> Table => _simpleTaskManagerRepository.Table;

        public virtual async Task<SimpleTaskManager> GetSimpleTaskManagerByIdAsync(int simpleTaskManagerId)
        {
            return await _simpleTaskManagerRepository.GetByIdAsync(simpleTaskManagerId);
        }

        public virtual async Task<IList<SimpleTaskManager>> GetSimpleTaskManagersByIdsAsync(int[] simpleTaskManagerIds)
        {
            return await _simpleTaskManagerRepository.GetByIdsAsync(simpleTaskManagerIds);
        }

        public virtual async Task<IList<SimpleTaskManager>> GetAllSimpleTaskManagersAsync()
        {
            var entities = await _simpleTaskManagerRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.CreatedDate);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteSimpleTaskManagerAsync(SimpleTaskManager simpleTaskManager)
        {
            await _simpleTaskManagerRepository.DeleteAsync(simpleTaskManager);
        }

        public virtual async Task DeleteSimpleTaskManagerAsync(IList<SimpleTaskManager> simpleTaskManagers)
        {
            await _simpleTaskManagerRepository.DeleteAsync(simpleTaskManagers);
        }

        public virtual async Task InsertSimpleTaskManagerAsync(SimpleTaskManager simpleTaskManager)
        {
            await _simpleTaskManagerRepository.InsertAsync(simpleTaskManager);
        }

        public virtual async Task UpdateSimpleTaskManagerAsync(SimpleTaskManager simpleTaskManager)
        {
            await _simpleTaskManagerRepository.UpdateAsync(simpleTaskManager);
        }
    }
}