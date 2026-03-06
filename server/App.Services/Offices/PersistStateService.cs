using App.Core;
using App.Core.Domain.Common;
using App.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Offices
{
    public partial interface IPersistStateService
    {
        IQueryable<PersistState> Table { get; }
        Task<PersistState> GetPersistStateByIdAsync(int persistStateId);
        Task<IList<PersistState>> GetPersistStatesByIdsAsync(int[] persistStateIds);
        Task<IList<PersistState>> GetAllPersistStatesAsync();
        Task DeletePersistStateAsync(PersistState persistState);
        Task DeletePersistStateAsync(IList<PersistState> persistStates);
        Task InsertPersistStateAsync(PersistState persistState);
        Task UpdatePersistStateAsync(PersistState persistState);
        Task<PersistState> GetPersistStateJsonValueAsync(Type type);
        Task<(T Model, bool Exist)> GetModelInstance<T>() where T : class, new();
    }
    public partial class PersistStateService : IPersistStateService
    {
        private readonly IRepository<PersistState> _persistStateRepository;
        private readonly IWorkContext _workContext;

        public PersistStateService(
            IRepository<PersistState> persistStateRepository, IWorkContext workContext)
        {
            _persistStateRepository = persistStateRepository;
            _workContext = workContext;
        }

        public virtual IQueryable<PersistState> Table => _persistStateRepository.Table;

        public virtual async Task<PersistState> GetPersistStateByIdAsync(int persistStateId)
        {
            return await _persistStateRepository.GetByIdAsync(persistStateId);
        }

        public virtual async Task<IList<PersistState>> GetPersistStatesByIdsAsync(int[] persistStateIds)
        {
            return await _persistStateRepository.GetByIdsAsync(persistStateIds);
        }

        public virtual async Task<IList<PersistState>> GetAllPersistStatesAsync()
        {
            var entities = await _persistStateRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Id);

                return query;
            });

            return entities;
        }

        public virtual async Task DeletePersistStateAsync(PersistState persistState)
        {
            await _persistStateRepository.DeleteAsync(persistState);
        }

        public virtual async Task DeletePersistStateAsync(IList<PersistState> persistStates)
        {
            await _persistStateRepository.DeleteAsync(persistStates);
        }

        public virtual async Task InsertPersistStateAsync(PersistState persistState)
        {
            await _persistStateRepository.InsertAsync(persistState);
        }

        public virtual async Task UpdatePersistStateAsync(PersistState persistState)
        {
            await _persistStateRepository.UpdateAsync(persistState);
        }

        public virtual async Task<PersistState> GetPersistStateJsonValueAsync(Type type)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            return await _persistStateRepository.Table
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customer.Id && x.ModelType == type.Name);
        }

        private async Task<PersistState> GetPersistStateByCurrentCustomerAsync<T>() where T : class
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            return await _persistStateRepository.Table
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customer.Id && x.ModelType == typeof(T).Name);
        }

        public virtual async Task<(T Model, bool Exist)> GetModelInstance<T>() where T : class, new()
        {
            T instance = new T();
            var persistState = await GetPersistStateByCurrentCustomerAsync<T>();
            var exist = persistState != null;

            if (exist)
            {
                try
                {
                    instance = JsonConvert.DeserializeObject<T>(persistState.JsonValue);
                }
                catch
                {
                    await DeletePersistStateAsync(persistState);
                    exist = false;
                }
            }

            return (instance, exist);
        }

    }
}