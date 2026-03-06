using App.Core.Domain.VatExemption;
using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.VatExemption
{
    public partial interface IVatExemptionReportService
    {
        IQueryable<VatExemptionReport> Table { get; }
        Task<VatExemptionReport> GetVatExemptionReportByIdAsync(int vatExemptionReportId);
        Task<IList<VatExemptionReport>> GetVatExemptionReportsByIdsAsync(int[] vatExemptionReportIds);
        Task<IList<VatExemptionReport>> GetAllVatExemptionReportsAsync(int traderId = 0);
        Task DeleteVatExemptionReportAsync(VatExemptionReport vatExemptionReport);
        Task DeleteVatExemptionReportAsync(IList<VatExemptionReport> vatExemptionReports);
        Task InsertVatExemptionReportAsync(VatExemptionReport vatExemptionReport);
        Task UpdateVatExemptionReportAsync(VatExemptionReport vatExemptionReport);
    }
    public partial class VatExemptionReportService : IVatExemptionReportService
    {
        private readonly IRepository<VatExemptionReport> _vatExemptionReportRepository;

        public VatExemptionReportService(
            IRepository<VatExemptionReport> vatExemptionReportRepository)
        {
            _vatExemptionReportRepository = vatExemptionReportRepository;
        }

        public virtual IQueryable<VatExemptionReport> Table => _vatExemptionReportRepository.Table;

        public virtual async Task<VatExemptionReport> GetVatExemptionReportByIdAsync(int vatExemptionReportId)
        {
            return await _vatExemptionReportRepository.GetByIdAsync(vatExemptionReportId);
        }

        public virtual async Task<IList<VatExemptionReport>> GetVatExemptionReportsByIdsAsync(int[] vatExemptionReportIds)
        {
            return await _vatExemptionReportRepository.GetByIdsAsync(vatExemptionReportIds);
        }

        public virtual async Task<IList<VatExemptionReport>> GetAllVatExemptionReportsAsync(int traderId = 0)
        {
            var entities = await _vatExemptionReportRepository.GetAllAsync(query =>
            {
                if (traderId > 0)
                    query = query.Where(x => x.TraderId == traderId);

                query = query.OrderBy(l => l.Id);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteVatExemptionReportAsync(VatExemptionReport vatExemptionReport)
        {
            await _vatExemptionReportRepository.DeleteAsync(vatExemptionReport);
        }

        public virtual async Task DeleteVatExemptionReportAsync(IList<VatExemptionReport> vatExemptionReports)
        {
            await _vatExemptionReportRepository.DeleteAsync(vatExemptionReports);
        }

        public virtual async Task InsertVatExemptionReportAsync(VatExemptionReport vatExemptionReport)
        {
            await _vatExemptionReportRepository.InsertAsync(vatExemptionReport);
        }

        public virtual async Task UpdateVatExemptionReportAsync(VatExemptionReport vatExemptionReport)
        {
            await _vatExemptionReportRepository.UpdateAsync(vatExemptionReport);
        }
    }
}