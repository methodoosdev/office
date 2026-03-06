using App.Core.Domain.Traders;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderRatingService
    {
        IQueryable<TraderRating> Table { get; }
        Task<TraderRating> GetTraderRatingByIdAsync(int traderRatingId);
        Task<IList<TraderRating>> GetTraderRatingByIdsAsync(int[] traderRatingIds);
        Task<IList<TraderRating>> GetAllTraderRatingAsync();
        Task DeleteTraderRatingAsync(TraderRating traderRating);
        Task DeleteTraderRatingAsync(IList<TraderRating> traderRatings);
        Task InsertTraderRatingAsync(TraderRating traderRating);
        Task UpdateTraderRatingAsync(TraderRating traderRating);
    }
    public partial class TraderRatingService : ITraderRatingService
    {
        private readonly IRepository<TraderRating> _traderRatingRepository;

        public TraderRatingService(
            IRepository<TraderRating> traderRatingRepository)
        {
            _traderRatingRepository = traderRatingRepository;
        }

        public virtual IQueryable<TraderRating> Table => _traderRatingRepository.Table;

        public virtual async Task<TraderRating> GetTraderRatingByIdAsync(int traderRatingId)
        {
            return await _traderRatingRepository.GetByIdAsync(traderRatingId);
        }

        public virtual async Task<IList<TraderRating>> GetTraderRatingByIdsAsync(int[] traderRatingIds)
        {
            return await _traderRatingRepository.GetByIdsAsync(traderRatingIds);
        }

        public virtual async Task<IList<TraderRating>> GetAllTraderRatingAsync()
        {
            var entities = await _traderRatingRepository.GetAllAsync(query =>
            {
                return query;
            });

            return entities;
        }

        public virtual async Task DeleteTraderRatingAsync(TraderRating traderRating)
        {
            await _traderRatingRepository.DeleteAsync(traderRating);
        }

        public virtual async Task DeleteTraderRatingAsync(IList<TraderRating> traderRatings)
        {
            await _traderRatingRepository.DeleteAsync(traderRatings);
        }

        public virtual async Task InsertTraderRatingAsync(TraderRating traderRating)
        {
            await _traderRatingRepository.InsertAsync(traderRating);
        }

        public virtual async Task UpdateTraderRatingAsync(TraderRating traderRating)
        {
            await _traderRatingRepository.UpdateAsync(traderRating);
        }
    }
}