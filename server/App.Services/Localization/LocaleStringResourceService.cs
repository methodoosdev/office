using App.Core;
using App.Core.Domain.Localization;
using App.Core.Infrastructure;
using App.Data;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Logging
{
    public partial interface ILocaleStringResourceService
    {
        IQueryable<LocaleStringResource> Table { get; }
        Task<LocaleStringResource> GetLocaleStringResourceByIdAsync(int localeStringResourceId);
        Task<IList<LocaleStringResource>> GetLocaleStringResourcesByIdsAsync(int[] localeStringResourceIds);
        Task<IList<LocaleStringResource>> GetAllLocaleStringResourcesAsync();
        Task<IPagedList<LocaleStringResourceModel>> GetPagedListAsync(LocaleStringResourceSearchModel searchModel,
            string systemKeyword = null, string resourceValue = null);
        Task DeleteLocaleStringResourceAsync(LocaleStringResource localeStringResource);
        Task DeleteLocaleStringResourceAsync(IList<LocaleStringResource> localeStringResources);
        Task InsertLocaleStringResourceAsync(LocaleStringResource localeStringResource);
        Task UpdateLocaleStringResourceAsync(LocaleStringResource localeStringResource);
    }
    public partial class LocaleStringResourceService : ILocaleStringResourceService
    {
        private readonly IRepository<LocaleStringResource> _localeStringResourceRepository;

        public LocaleStringResourceService(
            IRepository<LocaleStringResource> localeStringResourceRepository)
        {
            _localeStringResourceRepository = localeStringResourceRepository;
        }

        public virtual IQueryable<LocaleStringResource> Table => _localeStringResourceRepository.Table;

        public virtual async Task<LocaleStringResource> GetLocaleStringResourceByIdAsync(int localeStringResourceId)
        {
            return await _localeStringResourceRepository.GetByIdAsync(localeStringResourceId);
        }

        public virtual async Task<IList<LocaleStringResource>> GetLocaleStringResourcesByIdsAsync(int[] localeStringResourceIds)
        {
            return await _localeStringResourceRepository.GetByIdsAsync(localeStringResourceIds);
        }

        public virtual async Task<IList<LocaleStringResource>> GetAllLocaleStringResourcesAsync()
        {
            var entities = await _localeStringResourceRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.ResourceName);

                return query;
            });

            return entities;
        }

        public virtual async Task<IPagedList<LocaleStringResourceModel>> GetPagedListAsync(LocaleStringResourceSearchModel searchModel,
            string resourceName = null, string resourceValue = null)
        {
            var query = _localeStringResourceRepository.Table.AsEnumerable()
                .Select(x => x.ToModel<LocaleStringResourceModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.ResourceName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                     c.ResourceValue.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            if (!string.IsNullOrEmpty(resourceName))
                query = query.Where(poll => poll.ResourceName == resourceName);

            if (!string.IsNullOrEmpty(resourceValue))
                query = query.Where(poll => poll.ResourceValue == resourceValue);

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _localeStringResourceRepository.DeleteAsync(localeStringResource);
        }

        public virtual async Task DeleteLocaleStringResourceAsync(IList<LocaleStringResource> localeStringResources)
        {
            await _localeStringResourceRepository.DeleteAsync(localeStringResources);
        }

        public virtual async Task InsertLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _localeStringResourceRepository.InsertAsync(localeStringResource);
        }

        public virtual async Task UpdateLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _localeStringResourceRepository.UpdateAsync(localeStringResource);
        }
    }
}