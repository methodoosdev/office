using App.Core.Domain.Payroll;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Payroll
{
    public partial interface IWorkerScheduleShiftService
    {
        IQueryable<WorkerScheduleShift> Table { get; }
        Task<WorkerScheduleShift> GetWorkerScheduleShiftByIdAsync(int workerScheduleShiftId);
        Task<IList<WorkerScheduleShift>> GetWorkerScheduleShiftsByIdsAsync(int[] workerScheduleShiftIds);
        Task<IList<WorkerScheduleShift>> GetAllWorkerScheduleShiftsAsync(int traderId = 0);
        Task DeleteWorkerScheduleShiftAsync(WorkerScheduleShift workerScheduleShift);
        Task DeleteWorkerScheduleShiftAsync(IList<WorkerScheduleShift> workerScheduleShifts);
        Task InsertWorkerScheduleShiftAsync(WorkerScheduleShift workerScheduleShift);
        Task UpdateWorkerScheduleShiftAsync(WorkerScheduleShift workerScheduleShift);
    }
    public partial class WorkerScheduleShiftService : IWorkerScheduleShiftService
    {
        private readonly IRepository<WorkerScheduleShift> _workerScheduleShiftRepository;

        public WorkerScheduleShiftService(
            IRepository<WorkerScheduleShift> workerScheduleShiftRepository)
        {
            _workerScheduleShiftRepository = workerScheduleShiftRepository;
        }

        public virtual IQueryable<WorkerScheduleShift> Table => _workerScheduleShiftRepository.Table;

        public virtual async Task<WorkerScheduleShift> GetWorkerScheduleShiftByIdAsync(int workerScheduleShiftId)
        {
            return await _workerScheduleShiftRepository.GetByIdAsync(workerScheduleShiftId);
        }

        public virtual async Task<IList<WorkerScheduleShift>> GetWorkerScheduleShiftsByIdsAsync(int[] workerScheduleShiftIds)
        {
            return await _workerScheduleShiftRepository.GetByIdsAsync(workerScheduleShiftIds);
        }

        public virtual async Task<IList<WorkerScheduleShift>> GetAllWorkerScheduleShiftsAsync(int traderId = 0)
        {
            var entities = await _workerScheduleShiftRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                if (traderId > 0)
                    query = query.Where(x => x.TraderId == traderId);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteWorkerScheduleShiftAsync(WorkerScheduleShift workerScheduleShift)
        {
            await _workerScheduleShiftRepository.DeleteAsync(workerScheduleShift);
        }

        public virtual async Task DeleteWorkerScheduleShiftAsync(IList<WorkerScheduleShift> workerScheduleShifts)
        {
            await _workerScheduleShiftRepository.DeleteAsync(workerScheduleShifts);
        }

        public virtual async Task InsertWorkerScheduleShiftAsync(WorkerScheduleShift workerScheduleShift)
        {
            await _workerScheduleShiftRepository.InsertAsync(workerScheduleShift);
        }

        public virtual async Task UpdateWorkerScheduleShiftAsync(WorkerScheduleShift workerScheduleShift)
        {
            await _workerScheduleShiftRepository.UpdateAsync(workerScheduleShift);
        }
    }
}