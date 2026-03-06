using App.Core.Domain.Offices;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Offices
{
    public partial interface IChamberService
    {
        IQueryable<Chamber> Table { get; }
        Task<Chamber> GetChamberByIdAsync(int chamberId);
        Task<IList<Chamber>> GetChambersByIdsAsync(int[] chamberIds);
        Task<IList<Chamber>> GetAllChambersAsync();
        Task DeleteChamberAsync(Chamber chamber);
        Task DeleteChamberAsync(IList<Chamber> chambers);
        Task InsertChamberAsync(Chamber chamber);
        Task UpdateChamberAsync(Chamber chamber);
    }
    public partial class ChamberService : IChamberService
    {
        private readonly IRepository<Chamber> _chamberRepository;

        public ChamberService(
            IRepository<Chamber> chamberRepository)
        {
            _chamberRepository = chamberRepository;
        }

        public virtual IQueryable<Chamber> Table => _chamberRepository.Table;

        public virtual async Task<Chamber> GetChamberByIdAsync(int chamberId)
        {
            return await _chamberRepository.GetByIdAsync(chamberId);
        }

        public virtual async Task<IList<Chamber>> GetChambersByIdsAsync(int[] chamberIds)
        {
            return await _chamberRepository.GetByIdsAsync(chamberIds);
        }

        public virtual async Task<IList<Chamber>> GetAllChambersAsync()
        {
            var entities = await _chamberRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteChamberAsync(Chamber chamber)
        {
            await _chamberRepository.DeleteAsync(chamber);
        }

        public virtual async Task DeleteChamberAsync(IList<Chamber> chambers)
        {
            await _chamberRepository.DeleteAsync(chambers);
        }

        public virtual async Task InsertChamberAsync(Chamber chamber)
        {
            await _chamberRepository.InsertAsync(chamber);
        }

        public virtual async Task UpdateChamberAsync(Chamber chamber)
        {
            await _chamberRepository.UpdateAsync(chamber);
        }
    }
}