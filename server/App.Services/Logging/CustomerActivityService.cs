using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core;
using App.Core.Domain.Customers;
using App.Core.Domain.Employees;
using App.Core.Domain.Logging;
using App.Core.Infrastructure;
using App.Data;
using App.Services.Localization;

namespace App.Services.Logging
{
    public partial class CustomerActivityService : ICustomerActivityService
    {
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        public CustomerActivityService(IRepository<ActivityLog> activityLogRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _activityLogRepository = activityLogRepository;
            _activityLogTypeRepository = activityLogTypeRepository;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        public virtual async Task UpdateActivityTypeAsync(ActivityLogType activityLogType)
        {
            await _activityLogTypeRepository.UpdateAsync(activityLogType);
        }

        public virtual async Task<IList<ActivityLogType>> GetAllActivityTypesAsync()
        {
            var activityLogTypes = await _activityLogTypeRepository.GetAllAsync(query =>
            {
                return from alt in query
                       orderby alt.Name
                       select alt;
            }, cache => default);

            return activityLogTypes;
        }

        public virtual async Task<ActivityLogType> GetActivityTypeByIdAsync(int activityLogTypeId)
        {
            return await _activityLogTypeRepository.GetByIdAsync(activityLogTypeId, cache => default);
        }

        public virtual async Task<ActivityLog> InsertActivityOnceAsync(ActivityLogTypeType systemKeyword, string comment = null, BaseEntity entity = null)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var activityLogType = (await GetAllActivityTypesAsync()).FirstOrDefault(type => type.SystemKeyword.Equals(systemKeyword.ToString()));
            var createdOnUtc = DateTime.UtcNow;

            var exist = _activityLogRepository.Table.Any(x => x.CustomerId == customer.Id &&
                    x.ActivityLogTypeId == activityLogType.Id &&
                    x.CreatedOnUtc.Date == createdOnUtc.Date
                    );

            if ( exist)
                return null;

            return await InsertActivityAsync(customer, systemKeyword, comment, entity);
        }

        public virtual async Task<ActivityLog> InsertActivityAsync(ActivityLogTypeType systemKeyword, string comment = null, BaseEntity entity = null)
        {
            return await InsertActivityAsync(await _workContext.GetCurrentCustomerAsync(), systemKeyword, comment, entity);
        }

        public virtual async Task<ActivityLog> InsertActivityAsync(Customer customer, ActivityLogTypeType systemKeyword, string comment, BaseEntity entity = null)
        {
            if (customer == null)
                return null;

            //try to get activity log type by passed system keyword
            var activityLogType = (await GetAllActivityTypesAsync()).FirstOrDefault(type => type.SystemKeyword.Equals(systemKeyword.ToString()));
            if (!activityLogType?.Enabled ?? true)
                return null;

            //comment = await _localizationService.GetResourceAsync(comment, false);

            //insert log item
            var logItem = new ActivityLog
            {
                ActivityLogTypeId = activityLogType.Id,
                EntityId = entity?.Id,
                EntityName = entity?.GetType().Name,
                CustomerId = customer.Id,
                Comment = CommonHelper.EnsureMaximumLength(comment ?? string.Empty, 24000),
                CreatedOnUtc = DateTime.UtcNow,
                IpAddress = _webHelper.GetCurrentIpAddress()
            };
            await _activityLogRepository.InsertAsync(logItem);

            return logItem;
        }

        public virtual async Task DeleteActivityAsync(ActivityLog activityLog)
        {
            await _activityLogRepository.DeleteAsync(activityLog);
        }

        public virtual async Task DeleteActivityAsync(IList<ActivityLog> activityLogs)
        {
            await _activityLogRepository.DeleteAsync(activityLogs);
        }
        public virtual async Task<IPagedList<ActivityLog>> GetAllActivitiesAsync(DateTime? createdOnFrom = null, DateTime? createdOnTo = null,
            int? customerId = null, int? activityLogTypeId = null, string ipAddress = null, string entityName = null, int? entityId = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return await _activityLogRepository.GetAllPagedAsync(query =>
            {
                //filter by IP
                if (!string.IsNullOrEmpty(ipAddress))
                    query = query.Where(logItem => logItem.IpAddress.Contains(ipAddress));

                //filter by creation date
                if (createdOnFrom.HasValue)
                    query = query.Where(logItem => createdOnFrom.Value <= logItem.CreatedOnUtc);
                if (createdOnTo.HasValue)
                    query = query.Where(logItem => createdOnTo.Value >= logItem.CreatedOnUtc);

                //filter by log type
                if (activityLogTypeId.HasValue && activityLogTypeId.Value > 0)
                    query = query.Where(logItem => activityLogTypeId == logItem.ActivityLogTypeId);

                //filter by customer
                if (customerId.HasValue && customerId.Value > 0)
                    query = query.Where(logItem => customerId.Value == logItem.CustomerId);

                //filter by entity
                if (!string.IsNullOrEmpty(entityName))
                    query = query.Where(logItem => logItem.EntityName.Equals(entityName));
                if (entityId.HasValue && entityId.Value > 0)
                    query = query.Where(logItem => entityId.Value == logItem.EntityId);

                query = query.OrderByDescending(logItem => logItem.CreatedOnUtc).ThenBy(logItem => logItem.Id);

                return query;
            }, pageIndex, pageSize);
        }

        public virtual async Task<ActivityLog> GetActivityByIdAsync(int activityLogId)
        {
            return await _activityLogRepository.GetByIdAsync(activityLogId);
        }

        public virtual async Task<IList<ActivityLog>> GetActivitiesByIdsAsync(int[] activityLogIds)
        {
            return await _activityLogRepository.GetByIdsAsync(activityLogIds);
        }

        public virtual async Task ClearAllActivitiesAsync()
        {
            await _activityLogRepository.TruncateAsync();
        }

        public virtual async Task<IList<ActivityLog>> GetAllUserActivityAsync(DateTime fromDate, DateTime toDate)
        {
            var entities = await _activityLogRepository.GetAllAsync(query =>
            {                
                query = query.Where(x => x.CreatedOnUtc >= fromDate && x.CreatedOnUtc <= toDate);

                return query;
            });

            return entities;
        }
        public virtual IQueryable<ActivityLog> Table => _activityLogRepository.Table;

    }
}