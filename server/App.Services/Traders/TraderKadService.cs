using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderKadService
    {
        IQueryable<TraderKad> Table { get; }
        Task<TraderKad> GetTraderKadByIdAsync(int traderKadId);
        Task<IList<TraderKad>> GetTraderKadsByIdsAsync(int[] traderKadIds);
        Task<IList<TraderKad>> GetAllTraderKadsAsync(int traderId);
        Task<IPagedList<TraderKadModel>> GetPagedListAsync(TraderKadSearchModel searchModel, int traderId);
        Task DeleteTraderKadAsync(TraderKad traderKad);
        Task DeleteTraderKadAsync(IList<TraderKad> traderKads);
        Task InsertTraderKadAsync(TraderKad traderKad);
        Task InsertTraderKadAsync(IList<TraderKad> traderKads);
        Task UpdateTraderKadAsync(TraderKad traderKad);
    }
    public partial class TraderKadService : ITraderKadService
    {
        private readonly IRepository<TraderKad> _traderKadRepository;

        public TraderKadService(
            IRepository<TraderKad> traderKadRepository)
        {
            _traderKadRepository = traderKadRepository;
        }

        public virtual IQueryable<TraderKad> Table => _traderKadRepository.Table;

        public virtual async Task<TraderKad> GetTraderKadByIdAsync(int traderKadId)
        {
            return await _traderKadRepository.GetByIdAsync(traderKadId);
        }

        public virtual async Task<IList<TraderKad>> GetTraderKadsByIdsAsync(int[] traderKadIds)
        {
            return await _traderKadRepository.GetByIdsAsync(traderKadIds);
        }

        public virtual async Task<IList<TraderKad>> GetAllTraderKadsAsync(int traderId)
        {
            var entities = await _traderKadRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.TraderId == traderId);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<TraderKadModel>> GetPagedListAsync(TraderKadSearchModel searchModel, int traderId)
        {
            var query = _traderKadRepository.Table.AsEnumerable()
                .Select(x => x.ToModel<TraderKadModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => 
                    c.Code.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.Where(x => x.TraderId == traderId);

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteTraderKadAsync(TraderKad traderKad)
        {
            await _traderKadRepository.DeleteAsync(traderKad);
        }

        public virtual async Task DeleteTraderKadAsync(IList<TraderKad> traderKads)
        {
            await _traderKadRepository.DeleteAsync(traderKads);
        }

        public virtual async Task InsertTraderKadAsync(TraderKad traderKad)
        {
            await _traderKadRepository.InsertAsync(traderKad);
        }

        public virtual async Task InsertTraderKadAsync(IList<TraderKad> traderKads)
        {
            await _traderKadRepository.InsertAsync(traderKads);
        }

        public virtual async Task UpdateTraderKadAsync(TraderKad traderKad)
        {
            await _traderKadRepository.UpdateAsync(traderKad);
        }
    }
}