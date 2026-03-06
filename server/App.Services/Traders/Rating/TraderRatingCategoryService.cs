using App.Core.Domain.Traders;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderRatingCategoryService
    {
        IQueryable<TraderRatingCategory> Table { get; }
        Task<TraderRatingCategory> GetTraderRatingCategoryByIdAsync(int traderRatingCategoryId);
        Task<IList<TraderRatingCategory>> GetTraderRatingCategoriesByIdsAsync(int[] traderRatingCategoryIds);
        Task<IList<TraderRatingCategory>> GetAllTraderRatingCategoriesAsync();
        Task DeleteTraderRatingCategoryAsync(TraderRatingCategory traderRatingCategory);
        Task DeleteTraderRatingCategoryAsync(IList<TraderRatingCategory> traderRatingCategories);
        Task InsertTraderRatingCategoryAsync(TraderRatingCategory traderRatingCategory);
        Task UpdateTraderRatingCategoryAsync(TraderRatingCategory traderRatingCategory);
    }
    public partial class TraderRatingCategoryService : ITraderRatingCategoryService
    {
        private readonly IRepository<TraderRatingCategory> _traderRatingCategoryRepository;

        public TraderRatingCategoryService(
            IRepository<TraderRatingCategory> traderRatingCategoryRepository)
        {
            _traderRatingCategoryRepository = traderRatingCategoryRepository;
        }

        public virtual IQueryable<TraderRatingCategory> Table => _traderRatingCategoryRepository.Table;

        public virtual async Task<TraderRatingCategory> GetTraderRatingCategoryByIdAsync(int traderRatingCategoryId)
        {
            return await _traderRatingCategoryRepository.GetByIdAsync(traderRatingCategoryId);
        }

        public virtual async Task<IList<TraderRatingCategory>> GetTraderRatingCategoriesByIdsAsync(int[] traderRatingCategoryIds)
        {
            return await _traderRatingCategoryRepository.GetByIdsAsync(traderRatingCategoryIds);
        }

        public virtual async Task<IList<TraderRatingCategory>> GetAllTraderRatingCategoriesAsync()
        {
            var entities = await _traderRatingCategoryRepository.GetAllAsync(query =>
            {
                return query;
            });

            return entities;
        }

        public virtual async Task DeleteTraderRatingCategoryAsync(TraderRatingCategory traderRatingCategory)
        {
            await _traderRatingCategoryRepository.DeleteAsync(traderRatingCategory);
        }

        public virtual async Task DeleteTraderRatingCategoryAsync(IList<TraderRatingCategory> traderRatingCategories)
        {
            await _traderRatingCategoryRepository.DeleteAsync(traderRatingCategories);
        }

        public virtual async Task InsertTraderRatingCategoryAsync(TraderRatingCategory traderRatingCategory)
        {
            await _traderRatingCategoryRepository.InsertAsync(traderRatingCategory);
        }

        public virtual async Task UpdateTraderRatingCategoryAsync(TraderRatingCategory traderRatingCategory)
        {
            await _traderRatingCategoryRepository.UpdateAsync(traderRatingCategory);
        }
    }
}