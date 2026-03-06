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
    public partial interface ITraderMembershipService
    {
        IQueryable<TraderMembership> Table { get; }
        Task<TraderMembership> GetTraderMembershipByIdAsync(int traderMembershipId);
        Task<IList<TraderMembership>> GetTraderMembershipsByIdsAsync(int[] traderMembershipIds);
        Task<IList<TraderMembership>> GetAllTraderMembershipsAsync(int traderId);
        Task<IPagedList<TraderMembershipModel>> GetPagedListAsync(TraderMembershipSearchModel searchModel, int traderId);
        Task DeleteTraderMembershipAsync(TraderMembership traderMembership);
        Task DeleteTraderMembershipAsync(IList<TraderMembership> traderMemberships);
        Task InsertTraderMembershipAsync(TraderMembership traderMembership);
        Task InsertTraderMembershipAsync(IList<TraderMembership> traderMemberships);
        Task UpdateTraderMembershipAsync(TraderMembership traderMembership);
    }
    public partial class TraderMembershipService : ITraderMembershipService
    {
        private readonly IRepository<TraderMembership> _traderMembershipRepository;
        private readonly IRepository<TraderBoardMemberType> _traderBoardMemberTypeRepository;
        private readonly ITraderService _traderService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public TraderMembershipService(
            IRepository<TraderMembership> traderMembershipRepository,
            IRepository<TraderBoardMemberType> traderBoardMemberTypeRepository,
            ITraderService traderService,
            IDateTimeHelper dateTimeHelper)
        {
            _traderMembershipRepository = traderMembershipRepository;
            _traderBoardMemberTypeRepository = traderBoardMemberTypeRepository;
            _traderService = traderService;
            _dateTimeHelper = dateTimeHelper;
        }

        public virtual IQueryable<TraderMembership> Table => _traderMembershipRepository.Table;

        public virtual async Task<TraderMembership> GetTraderMembershipByIdAsync(int traderMembershipId)
        {
            return await _traderMembershipRepository.GetByIdAsync(traderMembershipId);
        }

        public virtual async Task<IList<TraderMembership>> GetTraderMembershipsByIdsAsync(int[] traderMembershipIds)
        {
            return await _traderMembershipRepository.GetByIdsAsync(traderMembershipIds);
        }

        public virtual async Task<IList<TraderMembership>> GetAllTraderMembershipsAsync(int traderId)
        {
            var entities = await _traderMembershipRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<TraderMembershipModel>> GetPagedListAsync(TraderMembershipSearchModel searchModel, int traderId)
        {
            var query = _traderMembershipRepository.Table
                .SelectAwait(async x =>
                {
                    DateTime? expireDateOn = null;
                    var traderBoardMemberType = await _traderBoardMemberTypeRepository.GetByIdAsync(x.TraderBoardMemberTypeId);
                    var trader = await _traderService.GetTraderByIdAsync(x.TraderId);

                    if (x.ExpireDateOnUtc.HasValue)
                        expireDateOn = await _dateTimeHelper.ConvertToUserTimeAsync(x.ExpireDateOnUtc.Value, DateTimeKind.Utc);

                    var model = x.ToModel<TraderMembershipModel>();
                    model.TraderName = trader.ToTraderFullName();
                    model.StartDateOn = await _dateTimeHelper.ConvertToUserTimeAsync(x.StartDateOnUtc, DateTimeKind.Utc);
                    model.ExpireDateOn = expireDateOn;
                    model.TraderBoardMemberTypeName = traderBoardMemberType.Name;

                    return model;
                });

            query = query.Where(x => x.TraderId == traderId);

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteTraderMembershipAsync(TraderMembership traderMembership)
        {
            await _traderMembershipRepository.DeleteAsync(traderMembership);
        }

        public virtual async Task DeleteTraderMembershipAsync(IList<TraderMembership> traderMemberships)
        {
            await _traderMembershipRepository.DeleteAsync(traderMemberships);
        }

        public virtual async Task InsertTraderMembershipAsync(TraderMembership traderMembership)
        {
            await _traderMembershipRepository.InsertAsync(traderMembership);
        }

        public virtual async Task InsertTraderMembershipAsync(IList<TraderMembership> traderMemberships)
        {
            await _traderMembershipRepository.InsertAsync(traderMemberships);
        }

        public virtual async Task UpdateTraderMembershipAsync(TraderMembership traderMembership)
        {
            await _traderMembershipRepository.UpdateAsync(traderMembership);
        }
    }
}