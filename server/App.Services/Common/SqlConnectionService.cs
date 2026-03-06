using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Services.Offices;
using System.Threading.Tasks;

namespace App.Services.Common
{
    public partial interface ISqlConnectionService
    {
        Task<ConnectionResult> GetConnectionAsync(SqlConnectionType type);
    }
    public partial class SqlConnectionService : ISqlConnectionService
    {
        private readonly IAccountingOfficeService _accountingOfficeService;
        private readonly IAppDataProvider _dataProvider;

        public SqlConnectionService(
            IAccountingOfficeService accountingOfficeService,
            IAppDataProvider dataProvider)
        {
            _accountingOfficeService = accountingOfficeService;
            _dataProvider = dataProvider;
        }

        #region Connections

        // Office
        private async Task<string> GetOfficeConnectionStringAsync()
        {
            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();
            var dataBaseName = office.OfficeDataBaseName?.Trim();
            var username = office.OfficeUsername?.Trim();
            var password = office.OfficePassword?.Trim();
            var ipAddress = office.OfficeIpAddress?.Trim();
            var port = office.OfficePort?.Trim();

            if (string.IsNullOrEmpty(dataBaseName) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(port))
            {
                return null;
            }

            var connection = _dataProvider.BuildConnectionString($"{ipAddress},{port}", dataBaseName, username, password);

            return connection;
        }

        // Srf
        private async Task<string> GetSrfConnectionStringAsync()
        {
            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();
            var dataBaseName = office.SrfDataBaseName?.Trim();
            var username = office.SrfUsername?.Trim();
            var password = office.SrfPassword?.Trim();
            var ipAddress = office.SrfIpAddress?.Trim();
            var port = office.SrfPort?.Trim();

            if (string.IsNullOrEmpty(dataBaseName) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(port))
            {
                return null;
            }

            var connection = _dataProvider.BuildConnectionString($"{ipAddress},{port}", dataBaseName, username, password);

            return connection;
        }

        // TaxSystem
        private async Task<string> GetTaxSystemConnectionStringAsync()
        {
            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();
            var dataBaseName = office.TaxSystemDataBaseName?.Trim();
            var username = office.TaxSystemUsername?.Trim();
            var password = office.TaxSystemPassword?.Trim();
            var ipAddress = office.TaxSystemIpAddress?.Trim();
            var port = office.TaxSystemPort?.Trim();

            if (string.IsNullOrEmpty(dataBaseName) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(port))
            {
                return null;
            }

            var connection = _dataProvider.BuildConnectionString($"{ipAddress},{port}", dataBaseName, username, password);

            return connection;
        }

        // HyperPayroll
        private async Task<string> GetHyperPayrollConnectionStringAsync()
        {
            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();
            var dataBaseName = office.HyperPayrollDataBaseName?.Trim();
            var username = office.HyperPayrollUsername?.Trim();
            var password = office.HyperPayrollPassword?.Trim();
            var ipAddress = office.HyperPayrollIpAddress?.Trim();
            var port = office.HyperPayrollPort?.Trim();

            if (string.IsNullOrEmpty(dataBaseName) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(port))
            {
                return null;
            }

            var connection = _dataProvider.BuildConnectionString($"{ipAddress},{port}", dataBaseName, username, password);

            return connection;
        }

        #endregion

        public virtual async Task<ConnectionResult> GetConnectionAsync(SqlConnectionType type)
        {
            var result = new ConnectionResult();

            var connection = type switch
            {
                SqlConnectionType.Office => await GetOfficeConnectionStringAsync(),
                SqlConnectionType.Srf => await GetSrfConnectionStringAsync(),
                SqlConnectionType.TaxSystem => await GetTaxSystemConnectionStringAsync(),
                SqlConnectionType.HyperM => await GetHyperPayrollConnectionStringAsync(),
                _ => throw new NopException($"Not supported data provider name: '{type}'"),
            };


            if (connection == null)
            {
                result.AddError("App.Errors.WrongCredentials"); return result;
            }

            if (!await _dataProvider.DatabaseExistsAsync(connection))
            {
                result.AddError("App.Errors.WrongConnection"); return result;
            }

            result.Connection = connection;

            return result;
        }
    }
}
