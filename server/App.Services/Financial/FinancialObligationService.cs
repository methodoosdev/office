using App.Core.Domain.Financial;
using App.Data;
using DocumentFormat.OpenXml.Bibliography;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Financial
{
    public partial interface IFinancialObligationService
    {
        IQueryable<FinancialObligation> Table { get; }
        Task<FinancialObligation> GetFinancialObligationByIdAsync(int financialObligationId);
        Task<IList<FinancialObligation>> GetFinancialObligationsByIdsAsync(int[] financialObligationIds);
        Task<IList<FinancialObligation>> GetAllFinancialObligationsAsync(int period, int year);
        Task DeleteFinancialObligationAsync(FinancialObligation financialObligation);
        Task DeleteFinancialObligationAsync(IList<FinancialObligation> financialObligations);
        Task InsertFinancialObligationAsync(FinancialObligation financialObligation);
        Task UpdateFinancialObligationAsync(FinancialObligation financialObligation);
        Task UpdateFinancialObligationAsync(IList<FinancialObligation> financialObligations);
    }
    public partial class FinancialObligationService : IFinancialObligationService
    {
        private readonly IRepository<FinancialObligation> _financialObligationRepository;

        public FinancialObligationService(
            IRepository<FinancialObligation> financialObligationRepository)
        {
            _financialObligationRepository = financialObligationRepository;
        }

        public virtual IQueryable<FinancialObligation> Table => _financialObligationRepository.Table;

        public virtual async Task<FinancialObligation> GetFinancialObligationByIdAsync(int financialObligationId)
        {
            return await _financialObligationRepository.GetByIdAsync(financialObligationId);
        }

        public virtual async Task<IList<FinancialObligation>> GetFinancialObligationsByIdsAsync(int[] financialObligationIds)
        {
            return await _financialObligationRepository.GetByIdsAsync(financialObligationIds);
        }

        public virtual async Task<IList<FinancialObligation>> GetAllFinancialObligationsAsync(int period, int year)
        {
            var entities = await _financialObligationRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.Period == period && x.CreatedOnUtc.Year == year);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteFinancialObligationAsync(FinancialObligation financialObligation)
        {
            await _financialObligationRepository.DeleteAsync(financialObligation);
        }

        public virtual async Task DeleteFinancialObligationAsync(IList<FinancialObligation> financialObligations)
        {
            await _financialObligationRepository.DeleteAsync(financialObligations);
        }

        public virtual async Task InsertFinancialObligationAsync(FinancialObligation financialObligation)
        {
            await _financialObligationRepository.InsertAsync(financialObligation);
        }

        public virtual async Task UpdateFinancialObligationAsync(FinancialObligation financialObligation)
        {
            await _financialObligationRepository.UpdateAsync(financialObligation);
        }
        public virtual async Task UpdateFinancialObligationAsync(IList<FinancialObligation> financialObligations)
        {
            await _financialObligationRepository.UpdateAsync(financialObligations);
        }

    }
}