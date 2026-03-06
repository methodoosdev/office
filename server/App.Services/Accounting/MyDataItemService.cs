using App.Core.Domain.Accounting;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Accounting
{
    public partial interface IMyDataItemService
    {
        IQueryable<MyDataItem> Table { get; }
        Task<MyDataItem> GetMyDataItemByIdAsync(int myDataItemId);
        Task<IList<MyDataItem>> GetMyDataItemsByIdsAsync(int[] myDataItemIds);
        Task<IList<MyDataItem>> GetAllMyDataItemAsync(string traderVat = null, int? docTypeId = null);
        Task DeleteMyDataItemAsync(MyDataItem myDataItem);
        Task DeleteMyDataItemAsync(IList<MyDataItem> myDataItems);
        Task InsertMyDataItemAsync(MyDataItem myDataItem);
        Task InsertMyDataItemAsync(IList<MyDataItem> myDataItems);
        Task UpdateMyDataItemAsync(MyDataItem myDataItem);
        bool Equals(MyDataItem obj1, MyDataItem obj2);
        Task<IList<MyDataItem>> GetMyDataItemByAsync(string traderVat, string vatNumber, string invoiceType,
            string series, int paymentMethodId);
    }
    public partial class MyDataItemService : IMyDataItemService
    {
        private readonly IRepository<MyDataItem> _myDataItemRepository;

        public MyDataItemService(
            IRepository<MyDataItem> myDataItemRepository)
        {
            _myDataItemRepository = myDataItemRepository;
        }

        public virtual IQueryable<MyDataItem> Table => _myDataItemRepository.Table;

        public virtual async Task<MyDataItem> GetMyDataItemByIdAsync(int myDataItemId)
        {
            return await _myDataItemRepository.GetByIdAsync(myDataItemId);
        }

        public virtual async Task<IList<MyDataItem>> GetMyDataItemsByIdsAsync(int[] myDataItemIds)
        {
            return await _myDataItemRepository.GetByIdsAsync(myDataItemIds);
        }

        public virtual async Task<IList<MyDataItem>> GetAllMyDataItemAsync(string traderVat = null, int? docTypeId = null)
        {
            var entities = await _myDataItemRepository.GetAllAsync(query =>
            {
                if (!string.IsNullOrEmpty(traderVat))
                    query = query.Where(x => x.TraderVat == traderVat);

                if (docTypeId.HasValue)
                    query = query.Where(x => x.DocTypeId == docTypeId.Value);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteMyDataItemAsync(MyDataItem myDataItem)
        {
            await _myDataItemRepository.DeleteAsync(myDataItem);
        }

        public virtual async Task DeleteMyDataItemAsync(IList<MyDataItem> myDataItems)
        {
            await _myDataItemRepository.DeleteAsync(myDataItems);
        }

        public virtual async Task InsertMyDataItemAsync(MyDataItem myDataItem)
        {
            await _myDataItemRepository.InsertAsync(myDataItem);
        }

        public virtual async Task InsertMyDataItemAsync(IList<MyDataItem> myDataItems)
        {
            await _myDataItemRepository.InsertAsync(myDataItems);
        }

        public virtual async Task UpdateMyDataItemAsync(MyDataItem myDataItem)
        {
            await _myDataItemRepository.UpdateAsync(myDataItem);
        }

        public bool Equals(MyDataItem obj1, MyDataItem obj2)
        {
            return 
                //obj1.LastDateOnUtc.Equals(obj2.LastDateOnUtc) &&
                string.Equals(obj1.TraderVat, obj2.TraderVat) &&
                string.Equals(obj1.CounterpartVat, obj2.CounterpartVat) &&
                string.Equals(obj1.InvoiceType, obj2.InvoiceType) &&
                obj1.Branch.Equals(obj2.Branch) &&
                obj1.PaymentMethodId.Equals(obj2.PaymentMethodId) &&
                obj1.VatProvisionId.Equals(obj2.VatProvisionId) &&
                obj1.SeriesId.Equals(obj2.SeriesId) &&
                obj1.DocTypeId.Equals(obj2.DocTypeId) &&
                obj1.IsIssuer.Equals(obj2.IsIssuer) &&
                obj1.VatCategoryId.Equals(obj2.VatCategoryId) &&
                obj1.TaxCategoryId.Equals(obj2.TaxCategoryId) &&
                string.Equals(obj1.ProductCode, obj2.ProductCode) &&
                obj1.VatId.Equals(obj2.VatId) &&
                obj1.CurrencyId.Equals(obj2.CurrencyId);
        }

        public bool Equals(MyDataItem item, string traderVat, string vatNumber, string invoiceType, string series,
            int paymentMethodId, int vatProvisionId, int vatCategoryId, int taxCategoryId)
        {
            return
                //obj1.LastDateOnUtc.Equals(obj2.LastDateOnUtc) &&
                string.Equals(item.TraderVat, traderVat) &&
                string.Equals(item.CounterpartVat, vatNumber) &&
                string.Equals(item.InvoiceType, invoiceType) &&
                string.Equals(item.Series, series) &&
                item.PaymentMethodId.Equals(paymentMethodId) &&
                item.VatProvisionId.Equals(vatProvisionId) &&
                item.VatCategoryId.Equals(vatCategoryId) &&
                item.TaxCategoryId.Equals(taxCategoryId);
        }

        public virtual async Task<MyDataItem> GetMyDataItemByAsync(string traderVat, string vatNumber, string invoiceType,
            string series, int paymentMethodId, int vatProvisionId, int vatCategoryId, int taxCategoryId)
        {
            var entities = await _myDataItemRepository.GetAllAsync(query =>
            {
                query = query.Where(x =>
                    string.Equals(x.TraderVat, traderVat) &&
                    string.Equals(x.CounterpartVat, vatNumber) &&
                    string.Equals(x.InvoiceType, invoiceType) &&
                    string.Equals(x.Series, series) &&
                    x.PaymentMethodId.Equals(paymentMethodId) &&
                    x.VatProvisionId.Equals(vatProvisionId) &&
                    x.VatCategoryId.Equals(vatCategoryId) &&
                    x.TaxCategoryId.Equals(taxCategoryId));

                return query;
            });

            return entities.FirstOrDefault();
        }

        public virtual async Task<IList<MyDataItem>> GetMyDataItemByAsync(string traderVat, string vatNumber, string invoiceType,
            string series, int paymentMethodId)
        {
            var entities = await _myDataItemRepository.GetAllAsync(query =>
            {
                query = query.AsEnumerable().Where(x =>
                    string.Equals(x.TraderVat, traderVat) &&
                    string.Equals(x.CounterpartVat, vatNumber) &&
                    string.Equals(x.InvoiceType, invoiceType) &&
                    string.Equals(x.Series, series) &&
                    x.PaymentMethodId.Equals(paymentMethodId)).AsQueryable();

                return query;
            });

            return entities;
        }
    }
}