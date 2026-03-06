using App.Core.Domain.Payroll;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Payroll
{
    public partial interface IWorkerLeaveDetailService
    {
        IQueryable<WorkerLeaveDetail> Table { get; }
        Task<WorkerLeaveDetail> GetWorkerLeaveDetailByIdAsync(int workerLeaveDetailId);
        Task<IList<WorkerLeaveDetail>> GetWorkerLeaveDetailsByIdsAsync(int[] workerLeaveDetailIds);
        Task<IList<WorkerLeaveDetail>> GetAllWorkerLeaveDetailsAsync(int traderId = 0);
        Task DeleteWorkerLeaveDetailAsync(WorkerLeaveDetail workerLeaveDetail);
        Task DeleteWorkerLeaveDetailAsync(IList<WorkerLeaveDetail> workerLeaveDetails);
        Task InsertWorkerLeaveDetailAsync(WorkerLeaveDetail workerLeaveDetail);
        Task UpdateWorkerLeaveDetailAsync(WorkerLeaveDetail workerLeaveDetail);
        Task UpdateWorkerLeaveDetailAsync(IList<WorkerLeaveDetail> workerLeaveDetail);
    }
    public partial class WorkerLeaveDetailService : IWorkerLeaveDetailService
    {
        private readonly IRepository<WorkerLeaveDetail> _workerLeaveDetailRepository;

        public WorkerLeaveDetailService(
            IRepository<WorkerLeaveDetail> workerLeaveDetailRepository)
        {
            _workerLeaveDetailRepository = workerLeaveDetailRepository;
        }

        public virtual IQueryable<WorkerLeaveDetail> Table => _workerLeaveDetailRepository.Table;

        public virtual async Task<WorkerLeaveDetail> GetWorkerLeaveDetailByIdAsync(int workerLeaveDetailId)
        {
            return await _workerLeaveDetailRepository.GetByIdAsync(workerLeaveDetailId);
        }

        public virtual async Task<IList<WorkerLeaveDetail>> GetWorkerLeaveDetailsByIdsAsync(int[] workerLeaveDetailIds)
        {
            return await _workerLeaveDetailRepository.GetByIdsAsync(workerLeaveDetailIds);
        }

        public virtual async Task<IList<WorkerLeaveDetail>> GetAllWorkerLeaveDetailsAsync(int traderId = 0)
        {
            var entities = await _workerLeaveDetailRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Id);

                if (traderId > 0)
                    query = query.Where(x => x.TraderId == traderId);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteWorkerLeaveDetailAsync(WorkerLeaveDetail workerLeaveDetail)
        {
            await _workerLeaveDetailRepository.DeleteAsync(workerLeaveDetail);
        }

        public virtual async Task DeleteWorkerLeaveDetailAsync(IList<WorkerLeaveDetail> workerLeaveDetails)
        {
            await _workerLeaveDetailRepository.DeleteAsync(workerLeaveDetails);
        }

        public virtual async Task InsertWorkerLeaveDetailAsync(WorkerLeaveDetail workerLeaveDetail)
        {
            await _workerLeaveDetailRepository.InsertAsync(workerLeaveDetail);
        }

        public virtual async Task UpdateWorkerLeaveDetailAsync(WorkerLeaveDetail workerLeaveDetail)
        {
            await _workerLeaveDetailRepository.UpdateAsync(workerLeaveDetail);
        }

        public virtual async Task UpdateWorkerLeaveDetailAsync(IList<WorkerLeaveDetail> workerLeaveDetails)
        {
            await _workerLeaveDetailRepository.UpdateAsync(workerLeaveDetails);
        }

    }
}