using App.Core.Domain.VatExemption;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.VatExemption
{
    public partial interface IVatExemptionSerialService
    {
        IQueryable<VatExemptionSerial> Table { get; }
        Task<VatExemptionSerial> GetVatExemptionSerialByIdAsync(int vatExemptionSerialId);
        Task<IList<VatExemptionSerial>> GetVatExemptionSerialsByIdsAsync(int[] vatExemptionSerialIds);
        Task<IList<VatExemptionSerial>> GetAllVatExemptionSerialsAsync(int traderId = 0);
        Task DeleteVatExemptionSerialAsync(VatExemptionSerial vatExemptionSerial);
        Task DeleteVatExemptionSerialAsync(IList<VatExemptionSerial> vatExemptionSerials);
        Task InsertVatExemptionSerialAsync(VatExemptionSerial vatExemptionSerial);
        Task UpdateVatExemptionSerialAsync(VatExemptionSerial vatExemptionSerial);
    }
    public partial class VatExemptionSerialService : IVatExemptionSerialService
    {
        private readonly IRepository<VatExemptionSerial> _vatExemptionSerialRepository;

        public VatExemptionSerialService(
            IRepository<VatExemptionSerial> vatExemptionSerialRepository)
        {
            _vatExemptionSerialRepository = vatExemptionSerialRepository;
        }

        public virtual IQueryable<VatExemptionSerial> Table => _vatExemptionSerialRepository.Table;

        public virtual async Task<VatExemptionSerial> GetVatExemptionSerialByIdAsync(int vatExemptionSerialId)
        {
            return await _vatExemptionSerialRepository.GetByIdAsync(vatExemptionSerialId);
        }

        public virtual async Task<IList<VatExemptionSerial>> GetVatExemptionSerialsByIdsAsync(int[] vatExemptionSerialIds)
        {
            return await _vatExemptionSerialRepository.GetByIdsAsync(vatExemptionSerialIds);
        }

        public virtual async Task<IList<VatExemptionSerial>> GetAllVatExemptionSerialsAsync(int traderId = 0)
        {
            var entities = await _vatExemptionSerialRepository.GetAllAsync(query =>
            {
                if (traderId > 0)
                    query = query.Where(x => x.TraderId == traderId);

                query = query.OrderBy(l => l.Id);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteVatExemptionSerialAsync(VatExemptionSerial vatExemptionSerial)
        {
            await _vatExemptionSerialRepository.DeleteAsync(vatExemptionSerial);
        }

        public virtual async Task DeleteVatExemptionSerialAsync(IList<VatExemptionSerial> vatExemptionSerials)
        {
            await _vatExemptionSerialRepository.DeleteAsync(vatExemptionSerials);
        }

        public virtual async Task InsertVatExemptionSerialAsync(VatExemptionSerial vatExemptionSerial)
        {
            await _vatExemptionSerialRepository.InsertAsync(vatExemptionSerial);
        }

        public virtual async Task UpdateVatExemptionSerialAsync(VatExemptionSerial vatExemptionSerial)
        {
            await _vatExemptionSerialRepository.UpdateAsync(vatExemptionSerial);
        }
    }
}