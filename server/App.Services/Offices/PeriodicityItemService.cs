using App.Core.Domain.Offices;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Offices
{
    public partial interface IPeriodicityItemService
    {
        IQueryable<PeriodicityItem> Table { get; }
        Task<PeriodicityItem> GetPeriodicityItemByIdAsync(int periodicityItemId);
        Task<IList<PeriodicityItem>> GetPeriodicityItemsByIdsAsync(int[] periodicityItemIds);
        Task<IList<PeriodicityItem>> GetAllPeriodicityItemsAsync();
        Task DeletePeriodicityItemAsync(PeriodicityItem periodicityItem);
        Task DeletePeriodicityItemAsync(IList<PeriodicityItem> periodicityItems);
        Task InsertPeriodicityItemAsync(PeriodicityItem periodicityItem);
        Task UpdatePeriodicityItemAsync(PeriodicityItem periodicityItem);
    }
    public partial class PeriodicityItemService : IPeriodicityItemService
    {
        private readonly IRepository<PeriodicityItem> _periodicityItemRepository;

        public PeriodicityItemService(
            IRepository<PeriodicityItem> periodicityItemRepository)
        {
            _periodicityItemRepository = periodicityItemRepository;
        }

        public virtual IQueryable<PeriodicityItem> Table => _periodicityItemRepository.Table;

        public virtual async Task<PeriodicityItem> GetPeriodicityItemByIdAsync(int periodicityItemId)
        {
            return await _periodicityItemRepository.GetByIdAsync(periodicityItemId);
        }

        public virtual async Task<IList<PeriodicityItem>> GetPeriodicityItemsByIdsAsync(int[] periodicityItemIds)
        {
            return await _periodicityItemRepository.GetByIdsAsync(periodicityItemIds);
        }

        public virtual async Task<IList<PeriodicityItem>> GetAllPeriodicityItemsAsync()
        {
            var entities = await _periodicityItemRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Paragraph);

                return query;
            });

            return entities;
        }

        public virtual async Task DeletePeriodicityItemAsync(PeriodicityItem periodicityItem)
        {
            await _periodicityItemRepository.DeleteAsync(periodicityItem);
        }

        public virtual async Task DeletePeriodicityItemAsync(IList<PeriodicityItem> periodicityItems)
        {
            await _periodicityItemRepository.DeleteAsync(periodicityItems);
        }

        public virtual async Task InsertPeriodicityItemAsync(PeriodicityItem periodicityItem)
        {
            await _periodicityItemRepository.InsertAsync(periodicityItem);
        }

        public virtual async Task UpdatePeriodicityItemAsync(PeriodicityItem periodicityItem)
        {
            await _periodicityItemRepository.UpdateAsync(periodicityItem);
        }
    }
}