using App.Core;
using App.Core.Domain.Traders;
using App.Data;
using App.Data.DataProviders;
using App.Models.Offices;
using App.Models.Traders;
using App.Services.Offices;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderConnectionService
    {
        //Task<TraderConnectionResult> _GetTraderSoftOneConnectionAsync(int traderId, bool checkCCategoryBooks = false);
        Task<TraderConnectionResult> GetTraderConnectionAsync(int traderId);
        Task<TraderConnectionResult> GetTraderAsync(int traderId);
    }
    public partial class TraderConnectionService : ITraderConnectionService
    {
        private readonly IRepository<Trader> _traderRepository;
        private readonly IAccountingOfficeService _accountingOfficeService;
        private readonly IAppDataProvider _dataProvider;

        public TraderConnectionService(
            IRepository<Trader> traderRepository,
            IAccountingOfficeService accountingOfficeService,
            IAppDataProvider dataProvider)
        {
            _traderRepository = traderRepository;
            _accountingOfficeService = accountingOfficeService;
            _dataProvider = dataProvider;
        }

        // Logistiki
        private string GetLogistikiConnectionString(TraderModel trader)
        {
            var dataBaseName = trader.LogistikiDataBaseName?.Trim();
            var username = trader.LogistikiUsername?.Trim();
            var password = trader.LogistikiPassword?.Trim();
            var ipAddress = trader.LogistikiIpAddress?.Trim();
            var port = trader.LogistikiPort?.Trim();

            if (string.IsNullOrEmpty(dataBaseName) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || (string.IsNullOrEmpty(ipAddress) && string.IsNullOrEmpty(port)))
            {
                return null;
            }

            var serverName = string.IsNullOrEmpty(port) ? ipAddress : $"{ipAddress},{port}";
            var connection = _dataProvider.BuildConnectionString(serverName, dataBaseName, username, password);

            return connection;
        }

        // HyperLog
        private async Task<string> GetHyperLogConnectionStringAsync()
        {
            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();
            var dataBaseName = office.HyperLogDataBaseName?.Trim();
            var username = office.HyperLogUsername?.Trim();
            var password = office.HyperLogPassword?.Trim();
            var ipAddress = office.HyperLogIpAddress?.Trim();
            var port = office.HyperLogPort?.Trim();

            if (string.IsNullOrEmpty(dataBaseName) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(port))
            {
                return null;
            }

            var serverName = string.IsNullOrEmpty(port) ? ipAddress : $"{ipAddress},{port}";
            var connection = _dataProvider.BuildConnectionString(serverName, dataBaseName, username, password);

            return connection;
        }

        // Prosvasis
        private async Task<string> GetProsvasisConnectionStringAsync()
        {
            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();
            var dataBaseName = office.ProsvasisDataBaseName?.Trim();
            var username = office.ProsvasisUsername?.Trim();
            var password = office.ProsvasisPassword?.Trim();
            var ipAddress = office.ProsvasisIpAddress?.Trim();
            var port = office.ProsvasisPort?.Trim();

            if (string.IsNullOrEmpty(dataBaseName) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(port))
            {
                return null;
            }

            var serverName = string.IsNullOrEmpty(port) ? ipAddress : $"{ipAddress},{port}";
            var connection = _dataProvider.BuildConnectionString(serverName, dataBaseName, username, password);

            return connection;
        }

        public virtual async Task<TraderConnectionResult> GetTraderConnectionAsync(int traderId)
        {
            var result = new TraderConnectionResult();

            var trader = await _traderRepository.GetByIdAsync(traderId);
            if (trader == null)
            {
                result.AddError("App.Errors.AccessDenied");
                return result;
            }

            var traderModel = trader.ToTraderModel();

            string connection = null;
            if (trader.LogistikiProgramType == LogistikiProgramType.SoftOne)
                connection = GetLogistikiConnectionString(traderModel);
            if (trader.LogistikiProgramType == LogistikiProgramType.HyperL)
                connection = await GetHyperLogConnectionStringAsync();
            if (trader.LogistikiProgramType == LogistikiProgramType.Prosvasis)
                connection = await GetProsvasisConnectionStringAsync();

            if (connection == null)
            {
                result.AddError("App.Errors.WrongCredentials"); return result;
            }

            if (!await _dataProvider.DatabaseExistsAsync(connection))
            {
                result.AddError("App.Errors.WrongConnection"); return result;
            }

            if (!(trader.CategoryBookType == CategoryBookType.C || trader.CategoryBookType == CategoryBookType.B))
            {
                result.AddError("App.Errors.WrongCategoryBooks"); return result;
            }

            result.TraderId = trader.Id;
            result.CompanyId = trader.CompanyId;
            result.CustomerTypeId = trader.CustomerTypeId;
            result.LogistikiProgramTypeId = trader.LogistikiProgramTypeId;
            result.CategoryBookTypeId = trader.CategoryBookTypeId;
            result.AccountingSchema = trader.AccountingSchema;
            result.HyperPayrollId = trader.HyperPayrollId;
            result.TraderPayment = trader.TraderPayment;
            result.TraderExpense = trader.TraderExpense;
            result.TaxesFee = trader.TaxesFee;
            result.Connection = connection;
            result.DiscountPredictions = trader.DiscountPredictions;
            result.DeductibleCredits = trader.DeductibleCredits;

            result.Vat = traderModel.Vat;
            result.TraderName = traderModel.FullName();
            result.TaxisUserName = traderModel.TaxisUserName;
            result.TaxisPassword = traderModel.TaxisPassword;
            result.MydataUserName = traderModel.MydataUserName;
            result.MydataPaswword = traderModel.MydataPaswword;

            return result;
        }
        public virtual async Task<TraderConnectionResult> GetTraderAsync(int traderId)
        {
            var result = new TraderConnectionResult();

            var trader = await _traderRepository.GetByIdAsync(traderId);
            if (trader == null)
            {
                result.AddError("App.Errors.AccessDenied");
                return result;
            }

            var traderModel = trader.ToTraderModel();

            if (!(trader.CategoryBookType == CategoryBookType.C || trader.CategoryBookType == CategoryBookType.B))
            {
                result.AddError("App.Errors.WrongCategoryBooks"); 
                return result;
            }

            result.TraderId = trader.Id;
            result.CompanyId = trader.CompanyId;
            result.CustomerTypeId = trader.CustomerTypeId;
            result.LogistikiProgramTypeId = trader.LogistikiProgramTypeId;
            result.CategoryBookTypeId = trader.CategoryBookTypeId;
            result.AccountingSchema = trader.AccountingSchema;
            result.HyperPayrollId = trader.HyperPayrollId;
            result.TraderPayment = trader.TraderPayment;
            result.TraderExpense = trader.TraderExpense;
            result.TaxesFee = trader.TaxesFee;
            result.DiscountPredictions = trader.DiscountPredictions;
            result.DeductibleCredits = trader.DeductibleCredits;

            result.Vat = traderModel.Vat;
            result.TraderName = traderModel.FullName();
            result.TaxisUserName = traderModel.TaxisUserName;
            result.TaxisPassword = traderModel.TaxisPassword;
            result.MydataUserName = traderModel.MydataUserName;
            result.MydataPaswword = traderModel.MydataPaswword;

            return result;
        }
    }
}
