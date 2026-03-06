using App.Core.Domain.Payroll;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Payroll
{
    public partial interface IWorkerScheduleDateService
    {
        IQueryable<WorkerScheduleDate> Table { get; }
        Task<WorkerScheduleDate> GetWorkerScheduleDateByIdAsync(int workerScheduleDateId);
        Task<IList<WorkerScheduleDate>> GetWorkerScheduleDatesByIdsAsync(int[] workerScheduleDateIds);
        Task<IList<WorkerScheduleDate>> GetAllWorkerScheduleDatesAsync(int parentId = 0);
        Task DeleteWorkerScheduleDateAsync(WorkerScheduleDate workerScheduleDate);
        Task DeleteWorkerScheduleDateAsync(IList<WorkerScheduleDate> workerScheduleDates);
        Task InsertWorkerScheduleDateAsync(WorkerScheduleDate workerScheduleDate);
        Task InsertWorkerScheduleDateAsync(IList<WorkerScheduleDate> workerScheduleDates);
        Task UpdateWorkerScheduleDateAsync(WorkerScheduleDate workerScheduleDate);
        Task UpdateWorkerScheduleDateAsync(IList<WorkerScheduleDate> workerScheduleDates);
    }
    public partial class WorkerScheduleDateService : IWorkerScheduleDateService
    {
        private readonly IRepository<WorkerScheduleDate> _workerScheduleDateRepository;

        public WorkerScheduleDateService(
            IRepository<WorkerScheduleDate> workerScheduleDateRepository)
        {
            _workerScheduleDateRepository = workerScheduleDateRepository;
        }

        public virtual IQueryable<WorkerScheduleDate> Table => _workerScheduleDateRepository.Table;

        public virtual async Task<WorkerScheduleDate> GetWorkerScheduleDateByIdAsync(int workerScheduleDateId)
        {
            return await _workerScheduleDateRepository.GetByIdAsync(workerScheduleDateId);
        }

        public virtual async Task<IList<WorkerScheduleDate>> GetWorkerScheduleDatesByIdsAsync(int[] workerScheduleDateIds)
        {
            return await _workerScheduleDateRepository.GetByIdsAsync(workerScheduleDateIds);
        }

        public virtual async Task<IList<WorkerScheduleDate>> GetAllWorkerScheduleDatesAsync(int parentId = 0)
        {
            var entities = await _workerScheduleDateRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.WorkingDate);

                if (parentId > 0)
                    query = query.Where(x => x.WorkerScheduleId == parentId);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteWorkerScheduleDateAsync(WorkerScheduleDate workerScheduleDate)
        {
            await _workerScheduleDateRepository.DeleteAsync(workerScheduleDate);
        }

        public virtual async Task DeleteWorkerScheduleDateAsync(IList<WorkerScheduleDate> workerScheduleDates)
        {
            await _workerScheduleDateRepository.DeleteAsync(workerScheduleDates);
        }

        public virtual async Task InsertWorkerScheduleDateAsync(WorkerScheduleDate workerScheduleDate)
        {
            await _workerScheduleDateRepository.InsertAsync(workerScheduleDate);
        }

        public virtual async Task InsertWorkerScheduleDateAsync(IList<WorkerScheduleDate> workerScheduleDates)
        {
            await _workerScheduleDateRepository.InsertAsync(workerScheduleDates);
        }

        public virtual async Task UpdateWorkerScheduleDateAsync(WorkerScheduleDate workerScheduleDate)
        {
            await _workerScheduleDateRepository.UpdateAsync(workerScheduleDate);
        }
        public virtual async Task UpdateWorkerScheduleDateAsync(IList<WorkerScheduleDate> workerScheduleDates)
        {
            await _workerScheduleDateRepository.UpdateAsync(workerScheduleDates);
        }

    }
}