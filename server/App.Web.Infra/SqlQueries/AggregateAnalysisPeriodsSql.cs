using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Data;
using LinqToDB.Data;
using System.Collections.Generic;
using System.Linq;

namespace App.Web.Infra.SqlQueries
{
    public class AggregateAnalysisPeriodsSql
    {
        public AggregateAnalysisPeriodsSql() { }

        public decimal Get(
            string connection, int companyId, int schema, int year, int fromPeriod, int toPeriod,
            IEnumerable<string> inc, IEnumerable<string> exc = null)
        {
            var builder = GetSql(companyId, schema, year, fromPeriod, toPeriod, inc, exc);

            var dataParams = builder.Parameters
                .Select(kvp => new DataParameter(kvp.Key, kvp.Value))
                .ToArray();

            var dataProvider = EngineContext.Current.Resolve<IAppDataProvider>();

            var value = dataProvider.QuerySimple<decimal>(connection, builder.Sql, dataParams);

            return value;
        }

        public decimal GetPayments(string connection, int companyId, int year, int fromPeriod, int toPeriod)
        {
            var builder = GetPaymentsSql(companyId, year, fromPeriod, toPeriod);

            var dataParams = builder.Parameters
                .Select(kvp => new DataParameter(kvp.Key, kvp.Value))
                .ToArray();

            var dataProvider = EngineContext.Current.Resolve<IAppDataProvider>();

            var value = dataProvider.QuerySimple<decimal>(connection, builder.Sql, dataParams);

            return value;
        }

        public decimal GetReceipts(string connection, int companyId, int year, int fromPeriod, int toPeriod)
        {
            var builder = GetReceiptsSql(companyId, year, fromPeriod, toPeriod);

            var dataParams = builder.Parameters
                .Select(kvp => new DataParameter(kvp.Key, kvp.Value))
                .ToArray();

            var dataProvider = EngineContext.Current.Resolve<IAppDataProvider>();

            var value = dataProvider.QuerySimple<decimal>(connection, builder.Sql, dataParams);

            return value;
        }

        public decimal GetOrders(string connection, int companyId, int year, int fromPeriod, int toPeriod)
        {
            var builder = GetOrdersSql(companyId, year, fromPeriod, toPeriod);

            var dataParams = builder.Parameters
                .Select(kvp => new DataParameter(kvp.Key, kvp.Value))
                .ToArray();

            var dataProvider = EngineContext.Current.Resolve<IAppDataProvider>();

            var value = dataProvider.QuerySimple<decimal>(connection, builder.Sql, dataParams);

            return value;
        }

        private (string Sql, Dictionary<string, object> Parameters) GetPaymentsSql(int companyId, int year, int fromPeriod, int toPeriod)
        {
            var columns = new[] { @"
				CAST(SUM(T.LLINEVAL) AS DECIMAL(18,2))
            " };

            var query = new SqlQueryBuilder()
                .Select(columns)
                .From(@"
				    FINDOC A LEFT OUTER JOIN 
				    TRDR B ON A.TRDR=B.TRDR LEFT OUTER JOIN 
				    TRDFLINES T ON T.FINDOC = A.FINDOC
                ")
                .Where("A.COMPANY = ?", companyId)
                .Where("AND A.FISCPRD = ?", year)
                .Where("AND (A.ISCANCEL IN (0,2))")
                .Where("AND ((A.SOSOURCE = 1281) OR (A.SOSOURCE=1681))")
                .Where("AND ((B.SODTYPE = 12) OR (B.SODTYPE = 16))")
                .AndWhereBetween("A.PERIOD", fromPeriod, toPeriod);

            return query.Build();
        }

        private (string Sql, Dictionary<string, object> Parameters) GetReceiptsSql(int companyId, int year, int fromPeriod, int toPeriod)
        {
            var columns = new[] { @"
				CAST(SUM(T.LLINEVAL) AS DECIMAL(18,2))
            " };

            var query = new SqlQueryBuilder()
                .Select(columns)
                .From(@"
				    FINDOC A LEFT OUTER JOIN 
				    TRDR B ON A.TRDR=B.TRDR LEFT OUTER JOIN 
				    TRDFLINES T ON T.FINDOC = A.FINDOC
                ")
                .Where("A.COMPANY = ?", companyId)
                .Where("AND A.FISCPRD = ?", year)
                .Where("AND (A.ISCANCEL IN (0,2))")
                .Where("AND ((A.SOSOURCE = 1381) OR (A.SOSOURCE=1581))")
                .Where("AND ((B.SODTYPE = 13) OR (B.SODTYPE = 15))")
                .AndWhereBetween("A.PERIOD", fromPeriod, toPeriod);

            return query.Build();
        }

        private (string Sql, Dictionary<string, object> Parameters) GetOrdersSql(int companyId, int year, int fromPeriod, int toPeriod)
        {
            var columns = new[] { @"
				CAST(SUM(A.NETAMNT) AS DECIMAL(18,2))
            " };

            var query = new SqlQueryBuilder()
                .Select(columns)
                .From("FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR")
                .Where("A.COMPANY = ?", companyId)
                .Where("AND A.FISCPRD = ?", year)
                .Where("AND A.SOSOURCE = 1351")
                .Where("AND A.SOREDIR = 0")
                .Where("AND A.TFPRMS = 201")
                .Where("AND A.SODTYPE = 13")
                .AndWhereBetween("A.PERIOD", fromPeriod, toPeriod);

            return query.Build();
        }

        private (string Sql, Dictionary<string, object> Parameters) GetSql(
            int companyId, int schema, int year, int fromPeriod, int toPeriod,
            IEnumerable<string> includePatterns, IEnumerable<string> excludePatterns)
        {
            var columns = new[] { @"
				CAST(
					SUM(
						CASE
						WHEN LEFT(A.CODE, 2) = '08' OR LEFT(A.CODE, 1) IN ('4','5','7') THEN B.LCREDITTMP - B.LDEBITTMP
						ELSE B.LDEBITTMP - B.LCREDITTMP
						END
					) 
				AS DECIMAL(18,2)
				)
            " };

            var query = new SqlQueryBuilder()
                .Select(columns)
                .From("ACNT A JOIN ACNBALSHEET B ON A.ACNT = B.ACNT")
                .Where("A.ACNSCHEMA = ?", schema)
                .Where("AND B.COMPANY = ?", companyId)
                .Where("AND B.FISCPRD = ?", year)
                .Where("AND A.SODTYPE = 89")
                .AndWhereBetween("B.PERIOD", fromPeriod, toPeriod);

            var inc = includePatterns?.ToList() ?? new List<string>();
            var exc = excludePatterns?.ToList() ?? new List<string>();

            if (inc.Count > 0)
                query.AndWhereLike("A.CODE", inc);

            if (exc.Count > 0)
                query.AndWhereNotLike("A.CODE", exc);

            return query.Build();
        }
    }
}
