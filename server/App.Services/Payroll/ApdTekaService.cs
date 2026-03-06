using App.Core.Domain.Payroll;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Payroll
{
    public partial interface IApdTekaService
    {
        IQueryable<ApdTeka> Table { get; }
        Task<ApdTeka> GetApdTekaByIdAsync(int apdTekaId);
        Task<IList<ApdTeka>> GetApdTekasByIdsAsync(int[] apdTekaIds);
        Task<IList<ApdTeka>> GetAllApdTekasAsync(int companyId = 0, int year = 0, int period = 0);
        Task DeleteApdTekaAsync(ApdTeka apdTeka);
        Task DeleteApdTekaAsync(IList<ApdTeka> apdTekas);
        Task InsertApdTekaAsync(ApdTeka apdTeka);
        Task InsertApdTekaAsync(IList<ApdTeka> apdTekas);
        Task UpdateApdTekaAsync(ApdTeka apdTeka);
        Task UpdateApdTekaAsync(IList<ApdTeka> apdTekas);
    }
    public partial class ApdTekaService : IApdTekaService
    {
        private readonly IRepository<ApdTeka> _apdTekaRepository;

        public ApdTekaService(
            IRepository<ApdTeka> apdTekaRepository)
        {
            _apdTekaRepository = apdTekaRepository;
        }

        public virtual IQueryable<ApdTeka> Table => _apdTekaRepository.Table;

        public virtual async Task<ApdTeka> GetApdTekaByIdAsync(int apdTekaId)
        {
            return await _apdTekaRepository.GetByIdAsync(apdTekaId);
        }

        public virtual async Task<IList<ApdTeka>> GetApdTekasByIdsAsync(int[] apdTekaIds)
        {
            return await _apdTekaRepository.GetByIdsAsync(apdTekaIds);
        }

        public virtual async Task<IList<ApdTeka>> GetAllApdTekasAsync(int companyId = 0, int year = 0, int period = 0)
        {
            var entities = await _apdTekaRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Id);

                if (companyId > 0)
                    query = query.Where(l => l.CompanyId == companyId);

                if (year > 0)
                    query = query.Where(l => l.Year == year);

                if (period > 0)
                    query = query.Where(l => l.Period == period);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteApdTekaAsync(ApdTeka apdTeka)
        {
            await _apdTekaRepository.DeleteAsync(apdTeka);
        }

        public virtual async Task DeleteApdTekaAsync(IList<ApdTeka> apdTekas)
        {
            await _apdTekaRepository.DeleteAsync(apdTekas);
        }

        public virtual async Task InsertApdTekaAsync(ApdTeka apdTeka)
        {
            await _apdTekaRepository.InsertAsync(apdTeka);
        }

        public virtual async Task InsertApdTekaAsync(IList<ApdTeka> apdTekas)
        {
            await _apdTekaRepository.InsertAsync(apdTekas);
        }

        public virtual async Task UpdateApdTekaAsync(ApdTeka apdTeka)
        {
            await _apdTekaRepository.UpdateAsync(apdTeka);
        }

        public virtual async Task UpdateApdTekaAsync(IList<ApdTeka> apdTekas)
        {
            await _apdTekaRepository.UpdateAsync(apdTekas);
        }
    }
}