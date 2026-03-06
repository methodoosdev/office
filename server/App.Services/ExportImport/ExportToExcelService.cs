using App.Core.Domain.Customers;
using App.Models.Accounting;
using App.Services.ExportImport.Help;
using App.Services.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Services.ExportImport
{
    public interface IExportToExcelService
    {
        Task<byte[]> CustomerToXlsxAsync(IEnumerable<Customer> list);
        Task<byte[]> ESendModelToXlsxAsync(IEnumerable<ESendModel> list);
        Task<byte[]> ArticlesCheckAccountModelToXlsxAsync(IEnumerable<ArticlesCheckAccountModel> list);
        Task<byte[]> MyDataExportToXlsxAsync(IEnumerable<MyDataExport> list);
    }
    public class ExportToExcelService : IExportToExcelService
    {
        private readonly ILocalizationService _localizationService;

        public ExportToExcelService(
            ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public virtual async Task<byte[]> CustomerToXlsxAsync(IEnumerable<Customer> list)
        {
            //property manager 
            var manager = new PropertyManager<Customer>(new[]
            {
                new PropertyByName<Customer>(await _localizationService.GetResourceAsync("App.Models.PrintAccountingModel.Columns.Username"), p => p.Username),
                new PropertyByName<Customer>(await _localizationService.GetResourceAsync("App.Models.PrintAccountingModel.Columns.Email"), p => p.Email)
            });

            return await manager.ExportToXlsxAsync(list);
        }

        public virtual async Task<byte[]> ESendModelToXlsxAsync(IEnumerable<ESendModel> list)
        {
            //property manager 
            var manager = new PropertyManager<ESendModel>(new[]
            {
                new PropertyByName<ESendModel>(await _localizationService.GetResourceAsync("App.Models.ESendModel.Columns.Date"), p => p.Date),
                new PropertyByName<ESendModel>(await _localizationService.GetResourceAsync("App.Models.ESendModel.Columns.Doc"), p => p.Doc),
                new PropertyByName<ESendModel>(await _localizationService.GetResourceAsync("App.Models.ESendModel.Columns.Payment"), p => p.Payment),
                new PropertyByName<ESendModel>(await _localizationService.GetResourceAsync("App.Models.ESendModel.Columns.Currency"), p => p.Currency),
                new PropertyByName<ESendModel>(await _localizationService.GetResourceAsync("App.Models.ESendModel.Columns.Value"), p => p.Value),
                new PropertyByName<ESendModel>(await _localizationService.GetResourceAsync("App.Models.ESendModel.Columns.Code"), p => p.Code),
                new PropertyByName<ESendModel>(await _localizationService.GetResourceAsync("App.Models.ESendModel.Columns.DocId"), p => p.DocId),
                new PropertyByName<ESendModel>(await _localizationService.GetResourceAsync("App.Models.ESendModel.Columns.CustomerCode"), p => p.CustomerCode),
                new PropertyByName<ESendModel>(await _localizationService.GetResourceAsync("App.Models.ESendModel.Columns.Number"), p => p.Number),
                new PropertyByName<ESendModel>(await _localizationService.GetResourceAsync("App.Models.ESendModel.Columns.Index"), p => p.Index)
            });

            return await manager.ExportToXlsxAsync(list);
        }

        public virtual async Task<byte[]> ArticlesCheckAccountModelToXlsxAsync(IEnumerable<ArticlesCheckAccountModel> list)
        {
            //property manager 
            var manager = new PropertyManager<ArticlesCheckAccountModel>(new[]
            {
                new PropertyByName<ArticlesCheckAccountModel>(await _localizationService.GetResourceAsync("App.Models.ArticlesCheckAccountModel.Columns.NglId"), p => p.NglId),
                new PropertyByName<ArticlesCheckAccountModel>(await _localizationService.GetResourceAsync("App.Models.ArticlesCheckAccountModel.Columns.NglName"), p => p.NglName),
                new PropertyByName<ArticlesCheckAccountModel>(await _localizationService.GetResourceAsync("App.Models.ArticlesCheckAccountModel.Columns.Code"), p => p.Code),
                new PropertyByName<ArticlesCheckAccountModel>(await _localizationService.GetResourceAsync("App.Models.ArticlesCheckAccountModel.Columns.Description"), p => p.Description),
                new PropertyByName<ArticlesCheckAccountModel>(await _localizationService.GetResourceAsync("App.Models.ArticlesCheckAccountModel.Columns.Debit"), p => p.Debit),
                new PropertyByName<ArticlesCheckAccountModel>(await _localizationService.GetResourceAsync("App.Models.ArticlesCheckAccountModel.Columns.Credit"), p => p.Credit),
                new PropertyByName<ArticlesCheckAccountModel>(await _localizationService.GetResourceAsync("App.Models.ArticlesCheckAccountModel.Columns.Date"), p => p.Date)
            });

            return await manager.ExportToXlsxAsync(list);
        }

        public virtual async Task<byte[]> MyDataExportToXlsxAsync(IEnumerable<MyDataExport> list)
        {
            //property manager 
            var manager = new PropertyManager<MyDataExport>(new[]
            {
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.SeriesId"), p => p.SeriesId),
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.CreatedOnUtc"), p => p.CreatedOnUtc),
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.Invoice"), p => p.Invoice),
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.TraderCode"), p => p.TraderCode),
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.ProductCode"), p => p.ProductCode),
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.Quantity"), p => p.Quantity),
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.NetValue"), p => p.NetValue),
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.VatId"), p => p.VatId),
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.CurrencyId"), p => p.CurrencyId),
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.VatProvisionId"), p => p.VatProvisionId),
                new PropertyByName<MyDataExport>(await _localizationService.GetResourceAsync("App.Models.MyDataExport.Columns.PaymentMethodId"), p => p.PaymentMethodId),
            });

            return await manager.ExportToXlsxAsync(list);
        }
    }
}
