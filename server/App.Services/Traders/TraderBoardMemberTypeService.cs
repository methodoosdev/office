using App.Core.Domain.Traders;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderBoardMemberTypeService
    {
        IQueryable<TraderBoardMemberType> Table { get; }
        Task<TraderBoardMemberType> GetTraderBoardMemberTypeByIdAsync(int traderBoardMemberTypeId);
        Task<IList<TraderBoardMemberType>> GetTraderBoardMemberTypesByIdsAsync(int[] traderBoardMemberTypeIds);
        Task<IList<TraderBoardMemberType>> GetAllTraderBoardMemberTypesAsync();
        Task DeleteTraderBoardMemberTypeAsync(TraderBoardMemberType traderBoardMemberType);
        Task InsertTraderBoardMemberTypeAsync(TraderBoardMemberType traderBoardMemberType);
        Task UpdateTraderBoardMemberTypeAsync(TraderBoardMemberType traderBoardMemberType);
    }
    public partial class TraderBoardMemberTypeService : ITraderBoardMemberTypeService
    {
        private readonly IRepository<TraderBoardMemberType> _traderBoardMemberTypeRepository;

        public TraderBoardMemberTypeService(IRepository<TraderBoardMemberType> traderBoardMemberTypeRepository)
        {
            _traderBoardMemberTypeRepository = traderBoardMemberTypeRepository;
        }

        public virtual IQueryable<TraderBoardMemberType> Table => _traderBoardMemberTypeRepository.Table;

        public virtual async Task<TraderBoardMemberType> GetTraderBoardMemberTypeByIdAsync(int traderBoardMemberTypeId)
        {
            return await _traderBoardMemberTypeRepository.GetByIdAsync(traderBoardMemberTypeId);
        }

        public virtual async Task<IList<TraderBoardMemberType>> GetTraderBoardMemberTypesByIdsAsync(int[] traderBoardMemberTypeIds)
        {
            return await _traderBoardMemberTypeRepository.GetByIdsAsync(traderBoardMemberTypeIds);
        }

        public virtual async Task<IList<TraderBoardMemberType>> GetAllTraderBoardMemberTypesAsync()
        {
            var entities = await _traderBoardMemberTypeRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(x => x.Name);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteTraderBoardMemberTypeAsync(TraderBoardMemberType traderBoardMemberType)
        {
            await _traderBoardMemberTypeRepository.DeleteAsync(traderBoardMemberType);
        }

        public virtual async Task InsertTraderBoardMemberTypeAsync(TraderBoardMemberType traderBoardMemberType)
        {
            await _traderBoardMemberTypeRepository.InsertAsync(traderBoardMemberType);
        }

        public virtual async Task UpdateTraderBoardMemberTypeAsync(TraderBoardMemberType traderBoardMemberType)
        {
            await _traderBoardMemberTypeRepository.UpdateAsync(traderBoardMemberType);
        }
    }
}