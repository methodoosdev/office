using App.Core;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Core.Domain.Traders;
using App.Data;
using App.Models.Traders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Infrastructure;

namespace App.Services.Traders
{
    public partial interface ITraderGroupService
    {
        IQueryable<TraderGroup> Table { get; }
        Task<TraderGroup> GetTraderGroupByIdAsync(int traderGroupId);
        Task<IList<TraderGroup>> GetTraderGroupsByIdsAsync(int[] traderGroupIds);
        Task<IList<TraderGroup>> GetAllTraderGroupsAsync();
        Task<IPagedList<TraderGroupModel>> GetPagedListAsync(TraderGroupSearchModel searchModel);
        Task DeleteTraderGroupAsync(TraderGroup traderGroup);
        Task DeleteTraderGroupAsync(IList<TraderGroup> traderGroups);
        Task InsertTraderGroupAsync(TraderGroup traderGroup);
        Task UpdateTraderGroupAsync(TraderGroup traderGroup);
    }
    public partial class TraderGroupService : ITraderGroupService
    {
        private readonly IRepository<TraderGroup> _traderGroupRepository;

        public TraderGroupService(
            IRepository<TraderGroup> traderGroupRepository)
        {
            _traderGroupRepository = traderGroupRepository;
        }

        public virtual IQueryable<TraderGroup> Table => _traderGroupRepository.Table;

        public virtual async Task<TraderGroup> GetTraderGroupByIdAsync(int traderGroupId)
        {
            return await _traderGroupRepository.GetByIdAsync(traderGroupId);
        }

        public virtual async Task<IList<TraderGroup>> GetTraderGroupsByIdsAsync(int[] traderGroupIds)
        {
            return await _traderGroupRepository.GetByIdsAsync(traderGroupIds);
        }

        public virtual async Task<IList<TraderGroup>> GetAllTraderGroupsAsync()
        {
            var entities = await _traderGroupRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<TraderGroupModel>> GetPagedListAsync(TraderGroupSearchModel searchModel)
        {
            var query = _traderGroupRepository.Table.AsEnumerable()
                .Select(x => x.ToModel<TraderGroupModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteTraderGroupAsync(TraderGroup traderGroup)
        {
            await _traderGroupRepository.DeleteAsync(traderGroup);
        }

        public virtual async Task DeleteTraderGroupAsync(IList<TraderGroup> traderGroups)
        {
            await _traderGroupRepository.DeleteAsync(traderGroups);
        }

        public virtual async Task InsertTraderGroupAsync(TraderGroup traderGroup)
        {
            await _traderGroupRepository.InsertAsync(traderGroup);
        }

        public virtual async Task UpdateTraderGroupAsync(TraderGroup traderGroup)
        {
            await _traderGroupRepository.UpdateAsync(traderGroup);
        }
    }
}