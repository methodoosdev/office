using App.Core.Domain.Traders;
using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    [Obsolete]
    public partial interface ITraderParentMappingService
    {
        Task<IList<Trader>> GetTradersByParentIdAsync(int parentId);
        Task<IList<Trader>> GetParentsByTraderIdAsync(int traderId);
        Task RemoveTraderParentAsync(Trader trader, Trader parent);
        Task InsertTraderParentAsync(Trader trader, Trader parent);
    }
    [Obsolete]
    public partial class TraderParentMappingService : ITraderParentMappingService
    {
        private readonly IRepository<Trader> _parentRepository;
        private readonly IRepository<Trader> _traderRepository;
        private readonly IRepository<TraderParentMapping> _traderParentMappingRepository;

        public TraderParentMappingService(
            IRepository<Trader> parentRepository,
            IRepository<Trader> traderRepository,
            IRepository<TraderParentMapping> traderParentMappingRepository)
        {
            _parentRepository = parentRepository;
            _traderRepository = traderRepository;
            _traderParentMappingRepository = traderParentMappingRepository;
        }

        public virtual async Task<IList<Trader>> GetTradersByParentIdAsync(int parentId)
        {
            return await _traderRepository.GetAllAsync(query =>
            {
                return from t in query
                       join tre in _traderParentMappingRepository.Table on t.Id equals tre.TraderId
                       where tre.ParentId == parentId
                       select t;
            });
        }

        public virtual async Task<IList<Trader>> GetParentsByTraderIdAsync(int traderId)
        {
            return await _parentRepository.GetAllAsync(query =>
            {
                return from e in query
                       join tre in _traderParentMappingRepository.Table on e.Id equals tre.ParentId
                       where tre.TraderId == traderId
                       select e;
            });
        }

        public virtual async Task RemoveTraderParentAsync(Trader trader, Trader parent)
        {
            if (trader == null)
                throw new ArgumentNullException(nameof(trader));

            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            if (await _traderParentMappingRepository.Table
                .FirstOrDefaultAsync(m => m.ParentId == parent.Id && m.TraderId == trader.Id) is TraderParentMapping mapping)
            {
                await _traderParentMappingRepository.DeleteAsync(mapping);
            }
        }

        public virtual async Task InsertTraderParentAsync(Trader trader, Trader parent)
        {
            if (trader is null)
                throw new ArgumentNullException(nameof(trader));

            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            if (await _traderParentMappingRepository.Table.FirstOrDefaultAsync(m => m.ParentId == parent.Id && m.TraderId == trader.Id) is null)
            {
                var mapping = new TraderParentMapping
                {
                    ParentId = parent.Id,
                    TraderId = trader.Id
                };

                await _traderParentMappingRepository.InsertAsync(mapping);
            }
        }

    }
}