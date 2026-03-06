using App.Core;
using App.Core.Domain.Logging;
using App.Data;
using App.Models.Logging;
using App.Framework.Infrastructure.Mapper.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Infrastructure;

namespace App.Services.Logging
{
    public partial interface IActivityLogTypeService
    {
        IQueryable<ActivityLogType> Table { get; }
        Task<ActivityLogType> GetActivityLogTypeByIdAsync(int activityLogTypeId);
        Task<IList<ActivityLogType>> GetActivityLogTypesByIdsAsync(int[] activityLogTypeIds);
        Task<IList<ActivityLogType>> GetAllActivityLogTypesAsync();
        Task<IPagedList<ActivityLogTypeModel>> GetPagedListAsync(ActivityLogTypeSearchModel searchModel,
            string systemKeyword = null);
        Task DeleteActivityLogTypeAsync(ActivityLogType activityLogType);
        Task DeleteActivityLogTypeAsync(IList<ActivityLogType> activityLogTypes);
        Task InsertActivityLogTypeAsync(ActivityLogType activityLogType);
        Task UpdateActivityLogTypeAsync(ActivityLogType activityLogType);
    }
    public partial class ActivityLogTypeService : IActivityLogTypeService
    {
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;

        public ActivityLogTypeService(
            IRepository<ActivityLogType> activityLogTypeRepository)
        {
            _activityLogTypeRepository = activityLogTypeRepository;
        }

        public virtual IQueryable<ActivityLogType> Table => _activityLogTypeRepository.Table;

        public virtual async Task<ActivityLogType> GetActivityLogTypeByIdAsync(int activityLogTypeId)
        {
            return await _activityLogTypeRepository.GetByIdAsync(activityLogTypeId);
        }

        public virtual async Task<IList<ActivityLogType>> GetActivityLogTypesByIdsAsync(int[] activityLogTypeIds)
        {
            return await _activityLogTypeRepository.GetByIdsAsync(activityLogTypeIds);
        }

        public virtual async Task<IList<ActivityLogType>> GetAllActivityLogTypesAsync()
        {
            var entities = await _activityLogTypeRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.SystemKeyword);

                return query;
            });

            return entities;
        }

        public virtual async Task<IPagedList<ActivityLogTypeModel>> GetPagedListAsync(ActivityLogTypeSearchModel searchModel,
            string systemKeyword = null)
        {
            var query = _activityLogTypeRepository.Table.AsEnumerable()
                .Select(x => x.ToModel<ActivityLogTypeModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.SystemKeyword.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            if (!string.IsNullOrEmpty(systemKeyword))
                query = query.Where(poll => poll.SystemKeyword == systemKeyword);

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteActivityLogTypeAsync(ActivityLogType activityLogType)
        {
            await _activityLogTypeRepository.DeleteAsync(activityLogType);
        }

        public virtual async Task DeleteActivityLogTypeAsync(IList<ActivityLogType> activityLogTypes)
        {
            await _activityLogTypeRepository.DeleteAsync(activityLogTypes);
        }

        public virtual async Task InsertActivityLogTypeAsync(ActivityLogType activityLogType)
        {
            await _activityLogTypeRepository.InsertAsync(activityLogType);
        }

        public virtual async Task UpdateActivityLogTypeAsync(ActivityLogType activityLogType)
        {
            await _activityLogTypeRepository.UpdateAsync(activityLogType);
        }
    }
}