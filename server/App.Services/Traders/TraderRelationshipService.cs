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
    public partial interface ITraderRelationshipService
    {
        IQueryable<TraderRelationship> Table { get; }
        Task<TraderRelationship> GetTraderRelationshipByIdAsync(int traderRelationshipId);
        Task<IList<TraderRelationship>> GetTraderRelationshipsByIdsAsync(int[] traderRelationshipIds);
        Task<IList<TraderRelationship>> GetAllTraderRelationshipsAsync(int traderId);
        Task<IPagedList<TraderRelationshipModel>> GetPagedListAsync(TraderRelationshipSearchModel searchModel, int traderId);
        Task DeleteTraderRelationshipAsync(TraderRelationship traderRelationship);
        Task DeleteTraderRelationshipAsync(IList<TraderRelationship> traderRelationships);
        Task InsertTraderRelationshipAsync(TraderRelationship traderRelationship);
        Task InsertTraderRelationshipAsync(IList<TraderRelationship> traderRelationships);
        Task UpdateTraderRelationshipAsync(TraderRelationship traderRelationship);
    }
    public partial class TraderRelationshipService : ITraderRelationshipService
    {
        private readonly IRepository<TraderRelationship> _traderRelationshipRepository;
        private readonly IRepository<TraderBoardMemberType> _traderBoardMemberTypeRepository;
        private readonly ITraderService _traderService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public TraderRelationshipService(
            IRepository<TraderRelationship> traderRelationshipRepository,
            IRepository<TraderBoardMemberType> traderBoardMemberTypeRepository,
            ITraderService traderService,
            IDateTimeHelper dateTimeHelper)
        {
            _traderRelationshipRepository = traderRelationshipRepository;
            _traderBoardMemberTypeRepository = traderBoardMemberTypeRepository;
            _traderService = traderService;
            _dateTimeHelper = dateTimeHelper;
        }

        public virtual IQueryable<TraderRelationship> Table => _traderRelationshipRepository.Table;

        public virtual async Task<TraderRelationship> GetTraderRelationshipByIdAsync(int traderRelationshipId)
        {
            return await _traderRelationshipRepository.GetByIdAsync(traderRelationshipId);
        }

        public virtual async Task<IList<TraderRelationship>> GetTraderRelationshipsByIdsAsync(int[] traderRelationshipIds)
        {
            return await _traderRelationshipRepository.GetByIdsAsync(traderRelationshipIds);
        }

        public virtual async Task<IList<TraderRelationship>> GetAllTraderRelationshipsAsync(int traderId)
        {
            var entities = await _traderRelationshipRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<TraderRelationshipModel>> GetPagedListAsync(TraderRelationshipSearchModel searchModel, int traderId)
        {
            var query = _traderRelationshipRepository.Table
                .SelectAwait(async x =>
                {
                    DateTime? expireDateOn = null;
                    var traderBoardMemberType = await _traderBoardMemberTypeRepository.GetByIdAsync(x.TraderBoardMemberTypeId);
                    var trader = await _traderService.GetTraderByIdAsync(x.TraderId);

                    if (x.ExpireDateOnUtc.HasValue)
                        expireDateOn = await _dateTimeHelper.ConvertToUserTimeAsync(x.ExpireDateOnUtc.Value, DateTimeKind.Utc);

                    var model = x.ToModel<TraderRelationshipModel>();
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

        public virtual async Task DeleteTraderRelationshipAsync(TraderRelationship traderRelationship)
        {
            await _traderRelationshipRepository.DeleteAsync(traderRelationship);
        }

        public virtual async Task DeleteTraderRelationshipAsync(IList<TraderRelationship> traderRelationships)
        {
            await _traderRelationshipRepository.DeleteAsync(traderRelationships);
        }

        public virtual async Task InsertTraderRelationshipAsync(TraderRelationship traderRelationship)
        {
            await _traderRelationshipRepository.InsertAsync(traderRelationship);
        }

        public virtual async Task InsertTraderRelationshipAsync(IList<TraderRelationship> traderRelationships)
        {
            await _traderRelationshipRepository.InsertAsync(traderRelationships);
        }

        public virtual async Task UpdateTraderRelationshipAsync(TraderRelationship traderRelationship)
        {
            await _traderRelationshipRepository.UpdateAsync(traderRelationship);
        }
    }
}