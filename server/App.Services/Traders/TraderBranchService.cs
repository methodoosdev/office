using App.Core.Domain.Traders;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderBranchService
    {
        IQueryable<TraderBranch> Table { get; }
        Task<TraderBranch> GetTraderBranchByIdAsync(int traderBranchId);
        Task<IList<TraderBranch>> GetTraderBranchsByIdsAsync(int[] traderBranchIds);
        Task<IList<TraderBranch>> GetAllTraderBranchesAsync(int traderId);
        Task DeleteTraderBranchAsync(TraderBranch traderBranch);
        Task DeleteTraderBranchAsync(IList<TraderBranch> traderBranchs);
        Task InsertTraderBranchAsync(TraderBranch traderBranch);
        Task InsertTraderBranchAsync(IList<TraderBranch> traderBranchs);
        Task UpdateTraderBranchAsync(TraderBranch traderBranch);
    }
    public partial class TraderBranchService : ITraderBranchService
    {
        private readonly IRepository<TraderBranch> _traderBranchRepository;

        public TraderBranchService(
            IRepository<TraderBranch> traderBranchRepository)
        {
            _traderBranchRepository = traderBranchRepository;
        }

        public virtual IQueryable<TraderBranch> Table => _traderBranchRepository.Table;

        public virtual async Task<TraderBranch> GetTraderBranchByIdAsync(int traderBranchId)
        {
            return await _traderBranchRepository.GetByIdAsync(traderBranchId);
        }

        public virtual async Task<IList<TraderBranch>> GetTraderBranchsByIdsAsync(int[] traderBranchIds)
        {
            return await _traderBranchRepository.GetByIdsAsync(traderBranchIds);
        }

        public virtual async Task<IList<TraderBranch>> GetAllTraderBranchesAsync(int traderId)
        {
            var entities = await _traderBranchRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteTraderBranchAsync(TraderBranch traderBranch)
        {
            await _traderBranchRepository.DeleteAsync(traderBranch);
        }

        public virtual async Task DeleteTraderBranchAsync(IList<TraderBranch> traderBranchs)
        {
            await _traderBranchRepository.DeleteAsync(traderBranchs);
        }

        public virtual async Task InsertTraderBranchAsync(TraderBranch traderBranch)
        {
            await _traderBranchRepository.InsertAsync(traderBranch);
        }

        public virtual async Task InsertTraderBranchAsync(IList<TraderBranch> traderBranchs)
        {
            await _traderBranchRepository.InsertAsync(traderBranchs);
        }

        public virtual async Task UpdateTraderBranchAsync(TraderBranch traderBranch)
        {
            await _traderBranchRepository.UpdateAsync(traderBranch);
        }
    }
}