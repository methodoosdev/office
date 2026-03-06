using App.Core.Domain.Accounting;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Accounting
{
    public partial interface IPeriodicF2Service
    {
        IQueryable<PeriodicF2> Table { get; }
        Task<PeriodicF2> GetPeriodicF2ByIdAsync(int periodicF2Id);
        Task<IList<PeriodicF2>> GetPeriodicF2ByIdsAsync(int[] periodicF2Ids);
        Task<IList<PeriodicF2>> GetAllPeriodicF2Async(int traderId = 0);
        Task DeletePeriodicF2Async(PeriodicF2 periodicF2);
        Task DeletePeriodicF2Async(IList<PeriodicF2> periodicF2s);
        Task InsertPeriodicF2Async(PeriodicF2 periodicF2);
        Task InsertPeriodicF2Async(IList<PeriodicF2> periodicF2s);
        Task UpdatePeriodicF2Async(PeriodicF2 periodicF2);
    }
    public partial class PeriodicF2Service : IPeriodicF2Service
    {
        private readonly IRepository<PeriodicF2> _periodicF2Repository;

        public PeriodicF2Service(
            IRepository<PeriodicF2> periodicF2Repository)
        {
            _periodicF2Repository = periodicF2Repository;
        }

        public virtual IQueryable<PeriodicF2> Table => _periodicF2Repository.Table;

        public virtual async Task<PeriodicF2> GetPeriodicF2ByIdAsync(int periodicF2Id)
        {
            return await _periodicF2Repository.GetByIdAsync(periodicF2Id);
        }

        public virtual async Task<IList<PeriodicF2>> GetPeriodicF2ByIdsAsync(int[] periodicF2Ids)
        {
            return await _periodicF2Repository.GetByIdsAsync(periodicF2Ids);
        }

        public virtual async Task<IList<PeriodicF2>> GetAllPeriodicF2Async(int traderId = 0)
        {
            var entities = await _periodicF2Repository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Id);

                if (traderId > 0)
                    query = query.Where(x => x.TraderId == traderId);

                return query;
            });

            return entities;
        }

        public virtual async Task DeletePeriodicF2Async(PeriodicF2 periodicF2)
        {
            await _periodicF2Repository.DeleteAsync(periodicF2);
        }

        public virtual async Task DeletePeriodicF2Async(IList<PeriodicF2> periodicF2s)
        {
            await _periodicF2Repository.DeleteAsync(periodicF2s);
        }

        public virtual async Task InsertPeriodicF2Async(PeriodicF2 periodicF2)
        {
            await _periodicF2Repository.InsertAsync(periodicF2);
        }

        public virtual async Task InsertPeriodicF2Async(IList<PeriodicF2> periodicF2s)
        {
            await _periodicF2Repository.InsertAsync(periodicF2s);
        }

        public virtual async Task UpdatePeriodicF2Async(PeriodicF2 periodicF2)
        {
            await _periodicF2Repository.UpdateAsync(periodicF2);
        }
    }
}