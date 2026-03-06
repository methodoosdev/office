using App.Core.Domain.Customers;
using App.Services.Caching;
using App.Services.Events;
using System.Threading.Tasks;

namespace App.Services.Customers.Caching
{
    /// <summary>
    /// Represents a customer cache event consumer
    /// </summary>
    public partial class CustomerCacheEventConsumer : CacheEventConsumer<Customer>, IConsumer<CustomerPasswordChangedEvent>
    {
        #region Methods

        /// <summary>
        /// Handle password changed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(CustomerPasswordChangedEvent eventMessage)
        {
            await RemoveAsync(NopCustomerServicesDefaults.CustomerPasswordLifetimeCacheKey, eventMessage.Password.CustomerId);
        }

        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Customer entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
            {
                await RemoveAsync(NopCustomerServicesDefaults.CustomerAddressesCacheKey, entity);
                await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerAddressesByCustomerPrefix, entity);
            }

            await base.ClearCacheAsync(entity, entityEventType);
        }

        #endregion
    }
}