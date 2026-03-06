using App.Core;
using App.Core.Domain.Traders;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Data;
using App.Models.Traders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Infrastructure;

namespace App.Services.Traders
{
    public partial interface IWorkingAreaService
    {
        IQueryable<WorkingArea> Table { get; }
        Task<WorkingArea> GetWorkingAreaByIdAsync(int workingAreaId);
        Task<IList<WorkingArea>> GetWorkingAreasByIdsAsync(int[] workingAreaIds);
        Task<IList<WorkingArea>> GetAllWorkingAreasAsync();
        Task<IPagedList<WorkingAreaModel>> GetPagedListAsync(WorkingAreaSearchModel searchModel);
        Task DeleteWorkingAreaAsync(WorkingArea workingArea);
        Task DeleteWorkingAreaAsync(IList<WorkingArea> workingAreas);
        Task InsertWorkingAreaAsync(WorkingArea workingArea);
        Task UpdateWorkingAreaAsync(WorkingArea workingArea);
    }
    public partial class WorkingAreaService : IWorkingAreaService
    {
        private readonly IRepository<WorkingArea> _workingAreaRepository;

        public WorkingAreaService(IRepository<WorkingArea> workingAreaRepository)
        {
            _workingAreaRepository = workingAreaRepository;
        }

        public virtual IQueryable<WorkingArea> Table => _workingAreaRepository.Table;

        public virtual async Task<WorkingArea> GetWorkingAreaByIdAsync(int workingAreaId)
        {
            return await _workingAreaRepository.GetByIdAsync(workingAreaId);
        }

        public virtual async Task<IList<WorkingArea>> GetWorkingAreasByIdsAsync(int[] workingAreaIds)
        {
            return await _workingAreaRepository.GetByIdsAsync(workingAreaIds);
        }

        public virtual async Task<IList<WorkingArea>> GetAllWorkingAreasAsync()
        {
            var entities = await _workingAreaRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<WorkingAreaModel>> GetPagedListAsync(WorkingAreaSearchModel searchModel)
        {
            var query = _workingAreaRepository.Table.AsEnumerable()
                .Select(x => x.ToModel<WorkingAreaModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteWorkingAreaAsync(WorkingArea workingArea)
        {
            await _workingAreaRepository.DeleteAsync(workingArea);
        }

        public virtual async Task DeleteWorkingAreaAsync(IList<WorkingArea> workingAreas)
        {
            await _workingAreaRepository.DeleteAsync(workingAreas);
        }

        public virtual async Task InsertWorkingAreaAsync(WorkingArea workingArea)
        {
            await _workingAreaRepository.InsertAsync(workingArea);
        }

        public virtual async Task UpdateWorkingAreaAsync(WorkingArea workingArea)
        {
            await _workingAreaRepository.UpdateAsync(workingArea);
        }
    }
}