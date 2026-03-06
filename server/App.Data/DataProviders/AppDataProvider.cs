using App.Core.Configuration;
using App.Data.DataProviders.Interceptors;
using App.Core.Infrastructure;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SqlServer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace App.Data.DataProviders
{
    public partial interface IAppDataProvider
    {
        Task<IList<T>> QueryAsync<T>(string connectionString, string sql);
        Task<IList<T>> QueryAsync<T>(string connectionString, string sql, params DataParameter[] parameters);
        Task<IList<T>> QueryTimeoutAsync<T>(string connectionString, int timeout, string sql, params DataParameter[] parameters);
        Task<T> QuerySimpleAsync<T>(string connectionString, string sql);
        Task<T> QuerySimpleAsync<T>(string connectionString, string sql, params DataParameter[] parameters);
        Task<bool> DatabaseExistsAsync(string connectionString);
        string BuildConnectionString(string serverName, string dataBaseName, string username, string password);
        T QuerySimple<T>(string connectionString, string sql);
        T QuerySimple<T>(string connectionString, string sql, params DataParameter[] parameters);
    }
    public partial class AppDataProvider : IAppDataProvider
    {
        protected static bool MiniProfillerEnabled => Singleton<AppSettings>.Instance.Get<CommonConfig>().MiniProfilerEnabled;

        protected IDataProvider LinqToDbDataProvider => SqlServerTools.GetDataProvider(SqlServerVersion.v2012, SqlServerProvider.MicrosoftDataSqlClient);

        protected DbConnection GetDbConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return new SqlConnection(connectionString);
        }

        protected virtual DataConnection CreateDataConnection(string connectionString)
        {
            var dataConnection = new DataConnection(LinqToDbDataProvider, GetDbConnection(connectionString))
            {
                CommandTimeout = DataSettingsManager.GetSqlCommandTimeout()
            };

            if (MiniProfillerEnabled)
            {
                dataConnection.AddInterceptor(UnwrapProfilerInterceptor.Instance);
            }

            return dataConnection;
        }

        protected virtual string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
        {
            if (nopConnectionString is null)
                throw new ArgumentNullException(nameof(nopConnectionString));

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = nopConnectionString.ServerName,
                InitialCatalog = nopConnectionString.DatabaseName,
                PersistSecurityInfo = false,
                IntegratedSecurity = nopConnectionString.IntegratedSecurity,
                TrustServerCertificate = true
            };

            if (!nopConnectionString.IntegratedSecurity)
            {
                builder.UserID = nopConnectionString.Username;
                builder.Password = nopConnectionString.Password;
            }

            return builder.ConnectionString;
        }

        public virtual Task<IList<T>> QueryAsync<T>(string connectionString, string sql)
        {
            using var dataContext = CreateDataConnection(connectionString);
            return Task.FromResult<IList<T>>(dataContext.Query<T>(sql)?.ToList() ?? new List<T>());
        }

        public virtual Task<IList<T>> QueryAsync<T>(string connectionString, string sql, params DataParameter[] parameters)
        {
            using var dataContext = CreateDataConnection(connectionString);
            return Task.FromResult<IList<T>>(dataContext.Query<T>(sql, parameters)?.ToList() ?? new List<T>());
        }

        public virtual Task<IList<T>> QueryTimeoutAsync<T>(string connectionString, int timeout, string sql, params DataParameter[] parameters)
        {
            using var dataContext = CreateDataConnection(connectionString);
            dataContext.CommandTimeout = timeout;
            return Task.FromResult<IList<T>>(dataContext.Query<T>(sql, parameters)?.ToList() ?? new List<T>());
        }

        public virtual Task<T> QuerySimpleAsync<T>(string connectionString, string sql)
        {
            using var dataContext = CreateDataConnection(connectionString);
            return Task.FromResult<T>(dataContext.Execute<T>(sql));
        }

        public virtual Task<T> QuerySimpleAsync<T>(string connectionString, string sql, params DataParameter[] parameters)
        {
            using var dataContext = CreateDataConnection(connectionString);
            return Task.FromResult<T>(dataContext.Execute<T>(sql, parameters));
        }

        public virtual T QuerySimple<T>(string connectionString, string sql)
        {
            using var dataContext = CreateDataConnection(connectionString);
            return dataContext.Execute<T>(sql);
        }

        public virtual T QuerySimple<T>(string connectionString, string sql, params DataParameter[] parameters)
        {
            using var dataContext = CreateDataConnection(connectionString);
            return dataContext.Execute<T>(sql, parameters);
        }

        public async Task<bool> DatabaseExistsAsync(string connectionString)
        {
            try
            {
                await using var connection = GetDbConnection(connectionString);

                //just try to connect
                await connection.OpenAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual string BuildConnectionString(string serverName, string dataBaseName, string username, string password)
        {
            if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(dataBaseName) ||
                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new NopException($"BuildConnectionString: Server:{serverName}, DB: {dataBaseName}, Username: {username}, Password: {password}");

            var info = new NopConnectionStringInfo
            {
                DatabaseName = dataBaseName,
                ServerName = serverName,
                IntegratedSecurity = false,
                Username = username,
                Password = password
            };

            return BuildConnectionString(info);
        }

    }
}
