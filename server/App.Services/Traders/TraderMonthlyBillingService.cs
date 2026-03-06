using App.Core.Domain.Traders;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderMonthlyBillingService
    {
        IQueryable<TraderMonthlyBilling> Table { get; }
        Task<TraderMonthlyBilling> GetTraderMonthlyBillingByIdAsync(int traderMonthlyBillingId);
        Task<IList<TraderMonthlyBilling>> GetTraderMonthlyBillingsByIdsAsync(int[] traderMonthlyBillingIds);
        Task<IList<TraderMonthlyBilling>> GetAllTraderMonthlyBillingsAsync(int traderId, int year = 0);
        Task DeleteTraderMonthlyBillingAsync(TraderMonthlyBilling traderMonthlyBilling);
        Task DeleteTraderMonthlyBillingAsync(IList<TraderMonthlyBilling> traderMonthlyBillings);
        Task InsertTraderMonthlyBillingAsync(TraderMonthlyBilling traderMonthlyBilling);
        Task InsertTraderMonthlyBillingAsync(IList<TraderMonthlyBilling> traderMonthlyBillings);
        Task UpdateTraderMonthlyBillingAsync(TraderMonthlyBilling traderMonthlyBilling);
    }
    public partial class TraderMonthlyBillingService : ITraderMonthlyBillingService
    {
        private readonly IRepository<TraderMonthlyBilling> _traderMonthlyBillingRepository;

        public TraderMonthlyBillingService(
            IRepository<TraderMonthlyBilling> traderMonthlyBillingRepository)
        {
            _traderMonthlyBillingRepository = traderMonthlyBillingRepository;
        }

        public virtual IQueryable<TraderMonthlyBilling> Table => _traderMonthlyBillingRepository.Table;

        public virtual async Task<TraderMonthlyBilling> GetTraderMonthlyBillingByIdAsync(int traderMonthlyBillingId)
        {
            return await _traderMonthlyBillingRepository.GetByIdAsync(traderMonthlyBillingId);
        }

        public virtual async Task<IList<TraderMonthlyBilling>> GetTraderMonthlyBillingsByIdsAsync(int[] traderMonthlyBillingIds)
        {
            return await _traderMonthlyBillingRepository.GetByIdsAsync(traderMonthlyBillingIds);
        }

        public virtual async Task<IList<TraderMonthlyBilling>> GetAllTraderMonthlyBillingsAsync(int traderId, int year = 0)
        {
            var entities = await _traderMonthlyBillingRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId);

                if (year > 0)
                    query = query.Where(x => x.Year == year);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteTraderMonthlyBillingAsync(TraderMonthlyBilling traderMonthlyBilling)
        {
            await _traderMonthlyBillingRepository.DeleteAsync(traderMonthlyBilling);
        }

        public virtual async Task DeleteTraderMonthlyBillingAsync(IList<TraderMonthlyBilling> traderMonthlyBillings)
        {
            await _traderMonthlyBillingRepository.DeleteAsync(traderMonthlyBillings);
        }

        public virtual async Task InsertTraderMonthlyBillingAsync(TraderMonthlyBilling traderMonthlyBilling)
        {
            await _traderMonthlyBillingRepository.InsertAsync(traderMonthlyBilling);
        }

        public virtual async Task InsertTraderMonthlyBillingAsync(IList<TraderMonthlyBilling> traderMonthlyBillings)
        {
            await _traderMonthlyBillingRepository.InsertAsync(traderMonthlyBillings);
        }

        public virtual async Task UpdateTraderMonthlyBillingAsync(TraderMonthlyBilling traderMonthlyBilling)
        {
            await _traderMonthlyBillingRepository.UpdateAsync(traderMonthlyBilling);
        }
    }
}