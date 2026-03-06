using App.Core.Domain.Payroll;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Payroll
{
    public partial interface IWorkerScheduleLogService
    {
        IQueryable<WorkerScheduleLog> Table { get; }
        Task<WorkerScheduleLog> GetWorkerScheduleLogByIdAsync(int workerScheduleLogId);
        Task<IList<WorkerScheduleLog>> GetWorkerScheduleLogsByIdsAsync(int[] workerScheduleLogIds);
        Task<IList<WorkerScheduleLog>> GetAllWorkerScheduleLogsAsync(int parentId = 0);
        Task DeleteWorkerScheduleLogAsync(WorkerScheduleLog workerScheduleLog);
        Task DeleteWorkerScheduleLogAsync(IList<WorkerScheduleLog> workerScheduleLogs);
        Task InsertWorkerScheduleLogAsync(WorkerScheduleLog workerScheduleLog);
        Task UpdateWorkerScheduleLogAsync(WorkerScheduleLog workerScheduleLog);
        Task UpdateWorkerScheduleLogAsync(IList<WorkerScheduleLog> workerScheduleLog);
    }
    public partial class WorkerScheduleLogService : IWorkerScheduleLogService
    {
        private readonly IRepository<WorkerScheduleLog> _workerScheduleLogRepository;

        public WorkerScheduleLogService(
            IRepository<WorkerScheduleLog> workerScheduleLogRepository)
        {
            _workerScheduleLogRepository = workerScheduleLogRepository;
        }

        public virtual IQueryable<WorkerScheduleLog> Table => _workerScheduleLogRepository.Table;

        public virtual async Task<WorkerScheduleLog> GetWorkerScheduleLogByIdAsync(int workerScheduleLogId)
        {
            return await _workerScheduleLogRepository.GetByIdAsync(workerScheduleLogId);
        }

        public virtual async Task<IList<WorkerScheduleLog>> GetWorkerScheduleLogsByIdsAsync(int[] workerScheduleLogIds)
        {
            return await _workerScheduleLogRepository.GetByIdsAsync(workerScheduleLogIds);
        }

        public virtual async Task<IList<WorkerScheduleLog>> GetAllWorkerScheduleLogsAsync(int parentId = 0)
        {
            var entities = await _workerScheduleLogRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Id);

                if (parentId > 0)
                    query = query.Where(x => x.WorkerScheduleId == parentId);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteWorkerScheduleLogAsync(WorkerScheduleLog workerScheduleLog)
        {
            await _workerScheduleLogRepository.DeleteAsync(workerScheduleLog);
        }

        public virtual async Task DeleteWorkerScheduleLogAsync(IList<WorkerScheduleLog> workerScheduleLogs)
        {
            await _workerScheduleLogRepository.DeleteAsync(workerScheduleLogs);
        }

        public virtual async Task InsertWorkerScheduleLogAsync(WorkerScheduleLog workerScheduleLog)
        {
            await _workerScheduleLogRepository.InsertAsync(workerScheduleLog);
        }

        public virtual async Task UpdateWorkerScheduleLogAsync(WorkerScheduleLog workerScheduleLog)
        {
            await _workerScheduleLogRepository.UpdateAsync(workerScheduleLog);
        }

        public virtual async Task UpdateWorkerScheduleLogAsync(IList<WorkerScheduleLog> workerScheduleLogs)
        {
            await _workerScheduleLogRepository.UpdateAsync(workerScheduleLogs);
        }

    }
}