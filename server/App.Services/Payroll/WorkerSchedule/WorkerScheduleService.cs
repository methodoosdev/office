using App.Core.Domain.Payroll;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Payroll
{
    public partial interface IWorkerScheduleService
    {
        IQueryable<WorkerSchedule> Table { get; }
        Task<WorkerSchedule> GetWorkerScheduleByIdAsync(int workerScheduleId);
        Task<IList<WorkerSchedule>> GetWorkerSchedulesByIdsAsync(int[] workerScheduleIds);
        Task<IList<WorkerSchedule>> GetAllWorkerSchedulesAsync();
        Task DeleteWorkerScheduleAsync(WorkerSchedule workerSchedule);
        Task DeleteWorkerScheduleAsync(IList<WorkerSchedule> workerSchedules);
        Task InsertWorkerScheduleAsync(WorkerSchedule workerSchedule);
        Task UpdateWorkerScheduleAsync(WorkerSchedule workerSchedule);
    }
    public partial class WorkerScheduleService : IWorkerScheduleService
    {
        private readonly IRepository<WorkerSchedule> _workerScheduleRepository;

        public WorkerScheduleService(
            IRepository<WorkerSchedule> workerScheduleRepository)
        {
            _workerScheduleRepository = workerScheduleRepository;
        }

        public virtual IQueryable<WorkerSchedule> Table => _workerScheduleRepository.Table;

        public virtual async Task<WorkerSchedule> GetWorkerScheduleByIdAsync(int workerScheduleId)
        {
            return await _workerScheduleRepository.GetByIdAsync(workerScheduleId);
        }

        public virtual async Task<IList<WorkerSchedule>> GetWorkerSchedulesByIdsAsync(int[] workerScheduleIds)
        {
            return await _workerScheduleRepository.GetByIdsAsync(workerScheduleIds);
        }

        public virtual async Task<IList<WorkerSchedule>> GetAllWorkerSchedulesAsync()
        {
            var entities = await _workerScheduleRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Id);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteWorkerScheduleAsync(WorkerSchedule workerSchedule)
        {
            await _workerScheduleRepository.DeleteAsync(workerSchedule);
        }

        public virtual async Task DeleteWorkerScheduleAsync(IList<WorkerSchedule> workerSchedules)
        {
            await _workerScheduleRepository.DeleteAsync(workerSchedules);
        }

        public virtual async Task InsertWorkerScheduleAsync(WorkerSchedule workerSchedule)
        {
            await _workerScheduleRepository.InsertAsync(workerSchedule);
        }

        public virtual async Task UpdateWorkerScheduleAsync(WorkerSchedule workerSchedule)
        {
            await _workerScheduleRepository.UpdateAsync(workerSchedule);
        }
    }
}