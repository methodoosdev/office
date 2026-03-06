using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderInfoService
    {
        IQueryable<TraderInfo> Table { get; }
        Task<TraderInfo> GetTraderInfoByIdAsync(int traderInfoId);
        Task<IList<TraderInfo>> GetTraderInfosByIdsAsync(int[] traderInfoIds);
        Task<IList<TraderInfo>> GetAllTraderInfosAsync(int traderId);
        Task<IPagedList<TraderInfoModel>> GetPagedListAsync(TraderInfoSearchModel searchModel, int traderId);
        Task DeleteTraderInfoAsync(TraderInfo traderInfo);
        Task DeleteTraderInfoAsync(IList<TraderInfo> traderInfos);
        Task InsertTraderInfoAsync(TraderInfo traderInfo);
        Task InsertTraderInfoAsync(IList<TraderInfo> traderInfos);
        Task UpdateTraderInfoAsync(TraderInfo traderInfo);
    }
    public partial class TraderInfoService : ITraderInfoService
    {
        private readonly IRepository<TraderInfo> _traderInfoRepository;
        private readonly IDateTimeHelper _dateTimeHelper;

        public TraderInfoService(
            IRepository<TraderInfo> traderInfoRepository,
            IDateTimeHelper dateTimeHelper)
        {
            _traderInfoRepository = traderInfoRepository;
            _dateTimeHelper = dateTimeHelper;
        }

        public virtual IQueryable<TraderInfo> Table => _traderInfoRepository.Table;

        public virtual async Task<TraderInfo> GetTraderInfoByIdAsync(int traderInfoId)
        {
            return await _traderInfoRepository.GetByIdAsync(traderInfoId);
        }

        public virtual async Task<IList<TraderInfo>> GetTraderInfosByIdsAsync(int[] traderInfoIds)
        {
            return await _traderInfoRepository.GetByIdsAsync(traderInfoIds);
        }

        public virtual async Task<IList<TraderInfo>> GetAllTraderInfosAsync(int traderId)
        {
            var entities = await _traderInfoRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<TraderInfoModel>> GetPagedListAsync(TraderInfoSearchModel searchModel, int traderId)
        {
            var query = _traderInfoRepository.Table
                .SelectAwait(async x => 
                {
                    var model = x.ToModel<TraderInfoModel>();
                    model.CreatedDate = await _dateTimeHelper.ConvertToUserTimeAsync(x.CreatedDate, DateTimeKind.Utc);

                    return model;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => 
                    c.SortDescription.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.Where(x => x.TraderId == traderId);

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteTraderInfoAsync(TraderInfo traderInfo)
        {
            await _traderInfoRepository.DeleteAsync(traderInfo);
        }

        public virtual async Task DeleteTraderInfoAsync(IList<TraderInfo> traderInfos)
        {
            await _traderInfoRepository.DeleteAsync(traderInfos);
        }

        public virtual async Task InsertTraderInfoAsync(TraderInfo traderInfo)
        {
            await _traderInfoRepository.InsertAsync(traderInfo);
        }

        public virtual async Task InsertTraderInfoAsync(IList<TraderInfo> traderInfos)
        {
            await _traderInfoRepository.InsertAsync(traderInfos);
        }

        public virtual async Task UpdateTraderInfoAsync(TraderInfo traderInfo)
        {
            await _traderInfoRepository.UpdateAsync(traderInfo);
        }
    }
}