using App.Core.Domain.Traders;
using App.Core.Domain.Traders.Rating;
using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderRatingTraderMappingService
    {
        IQueryable<TraderRatingTraderMapping> Table { get; }
        Task<IList<TraderRating>> GetTraderRatingByTraderIdAsync(int traderId);
        Task<IList<Trader>> GetTradersByTraderRatingIdAsync(int gravityId);
        Task RemoveTraderRatingTraderAsync(TraderRating gravity, Trader trader);
        Task InsertTraderRatingTraderAsync(TraderRating gravity, Trader trader);
    }
    public partial class TraderRatingTraderMappingService : ITraderRatingTraderMappingService
    {
        private readonly IRepository<Trader> _traderRepository;
        private readonly IRepository<TraderRating> _gravityRepository;
        private readonly IRepository<TraderRatingTraderMapping> _gravityTraderMappingRepository;

        public TraderRatingTraderMappingService(
            IRepository<Trader> traderRepository,
            IRepository<TraderRating> gravityRepository,
            IRepository<TraderRatingTraderMapping> gravityTraderMappingRepository)
        {
            _traderRepository = traderRepository;
            _gravityRepository = gravityRepository;
            _gravityTraderMappingRepository = gravityTraderMappingRepository;
        }

        public virtual IQueryable<TraderRatingTraderMapping> Table => _gravityTraderMappingRepository.Table;

        public virtual async Task<IList<TraderRating>> GetTraderRatingByTraderIdAsync(int traderId)
        {
            return await _gravityRepository.GetAllAsync(query =>
            {
                return from cp in query
                       join cpc in _gravityTraderMappingRepository.Table on cp.Id equals cpc.TraderRatingId
                       where cpc.TraderId == traderId
                       select cp;
            });
        }

        public virtual async Task<IList<Trader>> GetTradersByTraderRatingIdAsync(int gravityId)
        {
            return await _traderRepository.GetAllAsync(query =>
            {
                return from c in query
                       join cpc in _gravityTraderMappingRepository.Table on c.Id equals cpc.TraderId
                       where cpc.TraderRatingId == gravityId
                       select c;
            });
        }

        public virtual async Task RemoveTraderRatingTraderAsync(TraderRating gravity, Trader trader)
        {
            if (gravity == null)
                throw new ArgumentNullException(nameof(gravity));

            if (trader is null)
                throw new ArgumentNullException(nameof(trader));

            if (await _gravityTraderMappingRepository.Table
                .FirstOrDefaultAsync(x => x.TraderRatingId == gravity.Id && x.TraderId == trader.Id) is TraderRatingTraderMapping mapping)
            {
                await _gravityTraderMappingRepository.DeleteAsync(mapping);
            }
        }

        public virtual async Task InsertTraderRatingTraderAsync(TraderRating gravity, Trader trader)
        {
            if (gravity is null)
                throw new ArgumentNullException(nameof(gravity));

            if (trader is null)
                throw new ArgumentNullException(nameof(trader));

            if (await _gravityTraderMappingRepository.Table
                .FirstOrDefaultAsync(x => x.TraderRatingId == gravity.Id && x.TraderId == trader.Id) is null)
            {
                var mapping = new TraderRatingTraderMapping
                {
                    TraderRatingId = gravity.Id,
                    TraderId = trader.Id
                };

                await _gravityTraderMappingRepository.InsertAsync(mapping);
            }
        }

    }
}