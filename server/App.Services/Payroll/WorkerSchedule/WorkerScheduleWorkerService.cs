using App.Core.Domain.Payroll;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Payroll
{
    public partial interface IWorkerScheduleWorkerService
    {
        IQueryable<WorkerScheduleWorker> Table { get; }
        Task<WorkerScheduleWorker> GetWorkerScheduleWorkerByIdAsync(int workerScheduleWorkerId);
        Task<IList<WorkerScheduleWorker>> GetWorkerScheduleWorkersByIdsAsync(int[] workerScheduleWorkerIds);
        Task<IList<WorkerScheduleWorker>> GetAllWorkerScheduleWorkersAsync(int parentId = 0, bool onlyWorkers = false);
        Task DeleteWorkerScheduleWorkerAsync(WorkerScheduleWorker workerScheduleWorker);
        Task DeleteWorkerScheduleWorkerAsync(IList<WorkerScheduleWorker> workerScheduleWorkers);
        Task InsertWorkerScheduleWorkerAsync(WorkerScheduleWorker workerScheduleWorker);
        Task InsertWorkerScheduleWorkerAsync(IList<WorkerScheduleWorker> workerScheduleWorkers);
        Task UpdateWorkerScheduleWorkerAsync(WorkerScheduleWorker workerScheduleWorker);
    }
    public partial class WorkerScheduleWorkerService : IWorkerScheduleWorkerService
    {
        private readonly IRepository<WorkerScheduleWorker> _workerScheduleWorkerRepository;

        public WorkerScheduleWorkerService(
            IRepository<WorkerScheduleWorker> workerScheduleWorkerRepository)
        {
            _workerScheduleWorkerRepository = workerScheduleWorkerRepository;
        }

        public virtual IQueryable<WorkerScheduleWorker> Table => _workerScheduleWorkerRepository.Table;

        public virtual async Task<WorkerScheduleWorker> GetWorkerScheduleWorkerByIdAsync(int workerScheduleWorkerId)
        {
            return await _workerScheduleWorkerRepository.GetByIdAsync(workerScheduleWorkerId);
        }

        public virtual async Task<IList<WorkerScheduleWorker>> GetWorkerScheduleWorkersByIdsAsync(int[] workerScheduleWorkerIds)
        {
            return await _workerScheduleWorkerRepository.GetByIdsAsync(workerScheduleWorkerIds);
        }

        public virtual async Task<IList<WorkerScheduleWorker>> GetAllWorkerScheduleWorkersAsync(int parentId = 0, bool onlyWorkers = false)
        {
            var entities = await _workerScheduleWorkerRepository.GetAllAsync(query =>
            {
                if (parentId > 0)
                    query = query.Where(x => x.WorkerScheduleId == parentId);

                if (onlyWorkers)
                    query = query.Where(x => x.ActiveCard);

                query = query.OrderBy(x => x.Id);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteWorkerScheduleWorkerAsync(WorkerScheduleWorker workerScheduleWorker)
        {
            await _workerScheduleWorkerRepository.DeleteAsync(workerScheduleWorker);
        }

        public virtual async Task DeleteWorkerScheduleWorkerAsync(IList<WorkerScheduleWorker> workerScheduleWorkers)
        {
            await _workerScheduleWorkerRepository.DeleteAsync(workerScheduleWorkers);
        }

        public virtual async Task InsertWorkerScheduleWorkerAsync(WorkerScheduleWorker workerScheduleWorker)
        {
            await _workerScheduleWorkerRepository.InsertAsync(workerScheduleWorker);
        }

        public virtual async Task InsertWorkerScheduleWorkerAsync(IList<WorkerScheduleWorker> workerScheduleWorkers)
        {
            await _workerScheduleWorkerRepository.InsertAsync(workerScheduleWorkers);
        }

        public virtual async Task UpdateWorkerScheduleWorkerAsync(WorkerScheduleWorker workerScheduleWorker)
        {
            await _workerScheduleWorkerRepository.UpdateAsync(workerScheduleWorker);
        }
    }
}