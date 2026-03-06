using App.Core.Domain.SimpleTask;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.SimpleTask
{
    public partial interface ISimpleTaskSectorService
    {
        IQueryable<SimpleTaskSector> Table { get; }
        Task<SimpleTaskSector> GetSimpleTaskSectorByIdAsync(int simpleTaskSectorId);
        Task<IList<SimpleTaskSector>> GetSimpleTaskSectorsByIdsAsync(int[] simpleTaskSectorIds);
        Task<IList<SimpleTaskSector>> GetAllSimpleTaskSectorsAsync();
        Task DeleteSimpleTaskSectorAsync(SimpleTaskSector simpleTaskSector);
        Task DeleteSimpleTaskSectorAsync(IList<SimpleTaskSector> simpleTaskSectors);
        Task InsertSimpleTaskSectorAsync(SimpleTaskSector simpleTaskSector);
        Task UpdateSimpleTaskSectorAsync(SimpleTaskSector simpleTaskSector);
    }
    public partial class SimpleTaskSectorService : ISimpleTaskSectorService
    {
        private readonly IRepository<SimpleTaskSector> _simpleTaskSectorRepository;

        public SimpleTaskSectorService(
            IRepository<SimpleTaskSector> simpleTaskSectorRepository)
        {
            _simpleTaskSectorRepository = simpleTaskSectorRepository;
        }

        public virtual IQueryable<SimpleTaskSector> Table => _simpleTaskSectorRepository.Table;

        public virtual async Task<SimpleTaskSector> GetSimpleTaskSectorByIdAsync(int simpleTaskSectorId)
        {
            return await _simpleTaskSectorRepository.GetByIdAsync(simpleTaskSectorId);
        }

        public virtual async Task<IList<SimpleTaskSector>> GetSimpleTaskSectorsByIdsAsync(int[] simpleTaskSectorIds)
        {
            return await _simpleTaskSectorRepository.GetByIdsAsync(simpleTaskSectorIds);
        }

        public virtual async Task<IList<SimpleTaskSector>> GetAllSimpleTaskSectorsAsync()
        {
            var entities = await _simpleTaskSectorRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteSimpleTaskSectorAsync(SimpleTaskSector simpleTaskSector)
        {
            await _simpleTaskSectorRepository.DeleteAsync(simpleTaskSector);
        }

        public virtual async Task DeleteSimpleTaskSectorAsync(IList<SimpleTaskSector> simpleTaskSectors)
        {
            await _simpleTaskSectorRepository.DeleteAsync(simpleTaskSectors);
        }

        public virtual async Task InsertSimpleTaskSectorAsync(SimpleTaskSector simpleTaskSector)
        {
            await _simpleTaskSectorRepository.InsertAsync(simpleTaskSector);
        }

        public virtual async Task UpdateSimpleTaskSectorAsync(SimpleTaskSector simpleTaskSector)
        {
            await _simpleTaskSectorRepository.UpdateAsync(simpleTaskSector);
        }
    }
}