using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Framework.Data
{
    public class SqlQueryBuilder
    {
        private readonly List<string> _select = new();
        private string _from = "";
        private readonly List<string> _joins = new();
        private readonly List<string> _where = new();
        private string _groupBy;
        private readonly List<string> _having = new();
        private string _orderBy;
        private int? _limit;
        private int? _offset;
        private readonly List<string> _unions = new();
        private readonly List<string> _withClauses = new();
        private readonly Dictionary<string, object> _parameters = new();
        private int _paramCounter = 0;

        public SqlQueryBuilder() { }

        private SqlQueryBuilder(SqlQueryBuilder parent)
        {
            _paramCounter = parent._paramCounter;
        }

        public SqlQueryBuilder Select(params string[] columns)
        {
            _select.AddRange(columns);
            return this;
        }

        public SqlQueryBuilder SelectCase(Func<CaseBuilder, CaseBuilder> buildCase)
        {
            var caseBuilder = new CaseBuilder();
            var built = buildCase(caseBuilder);
            _select.Add(built.ToString());

            foreach (var kv in built.GetParameters())
                _parameters[kv.Key] = kv.Value;

            return this;
        }

        public SqlQueryBuilder From(string table)
        {
            _from = table;
            return this;
        }

        public SqlQueryBuilder FromSubquery(SqlQueryBuilder subqueryBuilder, string alias)
        {
            var (sql, subParams) = subqueryBuilder.Build();
            _from = $"({sql}) AS {alias}";

            foreach (var kv in subParams)
                _parameters[kv.Key] = kv.Value;

            return this;
        }

        public SqlQueryBuilder Join(string joinClause)
        {
            _joins.Add(joinClause);
            return this;
        }

        public SqlQueryBuilder JoinSubquery(SqlQueryBuilder subqueryBuilder, string alias, string onCondition)
        {
            var (sql, subParams) = subqueryBuilder.Build();
            _joins.Add($"JOIN ({sql}) AS {alias} ON {onCondition}");

            foreach (var kv in subParams)
                _parameters[kv.Key] = kv.Value;

            return this;
        }

        public SqlQueryBuilder With(string alias, SqlQueryBuilder subqueryBuilder)
        {
            var (sql, subParams) = subqueryBuilder.Build();
            _withClauses.Add($"{alias} AS ({sql})");

            foreach (var kv in subParams)
                _parameters[kv.Key] = kv.Value;

            return this;
        }

        public SqlQueryBuilder Where(string condition, object value = null)
        {
            if (value != null)
            {
                string paramName = $"@p{_paramCounter++}";
                _where.Add(condition.Replace("?", paramName));
                _parameters[paramName] = value;
            }
            else
            {
                _where.Add(condition);
            }
            return this;
        }

        public SqlQueryBuilder OrWhere(string condition, object value = null)
        {
            if (value != null)
            {
                string paramName = $"@p{_paramCounter++}";
                _where.Add($"OR {condition.Replace("?", paramName)}");
                _parameters[paramName] = value;
            }
            else
            {
                _where.Add("OR " + condition);
            }
            return this;
        }

        public SqlQueryBuilder WhereNull(string column)
        {
            _where.Add($"{column} IS NULL");
            return this;
        }

        public SqlQueryBuilder WhereNotNull(string column)
        {
            _where.Add($"{column} IS NOT NULL");
            return this;
        }

        public SqlQueryBuilder WhereBetween<T>(string column, T start, T end)
        {
            string paramStart = $"@p{_paramCounter++}";
            string paramEnd = $"@p{_paramCounter++}";
            _parameters[paramStart] = start;
            _parameters[paramEnd] = end;
            _where.Add($"{column} BETWEEN {paramStart} AND {paramEnd}");
            return this;
        }

        public SqlQueryBuilder AndWhereBetween<T>(string column, T start, T end)
        {
            string paramStart = $"@p{_paramCounter++}";
            string paramEnd = $"@p{_paramCounter++}";
            _parameters[paramStart] = start;
            _parameters[paramEnd] = end;
            _where.Add("AND " + $"{column} BETWEEN {paramStart} AND {paramEnd}");
            return this;
        }

        public SqlQueryBuilder WhereIn<T>(string column, IEnumerable<T> values)
        {
            var list = values.ToList();
            if (!list.Any()) return this;

            var paramNames = new List<string>();
            foreach (var value in list)
            {
                string paramName = $"@p{_paramCounter++}";
                paramNames.Add(paramName);
                _parameters[paramName] = value;
            }
            _where.Add($"{column} IN ({string.Join(", ", paramNames)})");
            return this;
        }

        public SqlQueryBuilder OrWhereIn<T>(string column, IEnumerable<T> values)
        {
            var list = values.ToList();
            if (!list.Any()) return this;

            var paramNames = new List<string>();
            foreach (var value in list)
            {
                string paramName = $"@p{_paramCounter++}";
                paramNames.Add(paramName);
                _parameters[paramName] = value;
            }
            _where.Add($"OR {column} IN ({string.Join(", ", paramNames)})");
            return this;
        }

        public SqlQueryBuilder WhereLike(string column, string pattern)
        {
            string paramName = $"@p{_paramCounter++}";
            _where.Add($"{column} LIKE {paramName}");
            _parameters[paramName] = pattern;
            return this;
        }

        public SqlQueryBuilder WhereLike(string column, IEnumerable<string> patterns)
        {
            var patternList = patterns.ToList();
            if (!patternList.Any()) return this;

            var likeConditions = new List<string>();
            foreach (var pattern in patternList)
            {
                string paramName = $"@p{_paramCounter++}";
                _parameters[paramName] = pattern;
                likeConditions.Add($"{column} LIKE {paramName}");
            }

            _where.Add("(" + string.Join(" OR ", likeConditions) + ")");
            return this;
        }

        public SqlQueryBuilder OrWhereLike(string column, IEnumerable<string> patterns)
        {
            var patternList = patterns.ToList();
            if (!patternList.Any()) return this;

            var likeConditions = new List<string>();
            foreach (var pattern in patternList)
            {
                string paramName = $"@p{_paramCounter++}";
                _parameters[paramName] = pattern;
                likeConditions.Add($"{column} LIKE {paramName}");
            }

            _where.Add("OR (" + string.Join(" OR ", likeConditions) + ")");
            return this;
        }

        public SqlQueryBuilder AndWhereLike(string column, IEnumerable<string> patterns)
        {
            var patternList = patterns.ToList();
            if (!patternList.Any()) return this;

            var likeConditions = new List<string>();
            foreach (var pattern in patternList)
            {
                string paramName = $"@p{_paramCounter++}";
                _parameters[paramName] = pattern;
                likeConditions.Add($"{column} LIKE {paramName}");
            }

            _where.Add("AND (" + string.Join(" OR ", likeConditions) + ")");
            return this;
        }
        public SqlQueryBuilder AndWhereNotLike(string column, IEnumerable<string> patterns)
        {
            var patternList = patterns?.ToList() ?? new List<string>();
            if (patternList.Count == 0) return this;

            var notLikeConditions = new List<string>();
            foreach (var pattern in patternList)
            {
                string paramName = $"@p{_paramCounter++}";
                _parameters[paramName] = pattern;
                notLikeConditions.Add($"{column} NOT LIKE {paramName}");
            }

            // ALL must be true to exclude any match -> join with AND
            _where.Add("AND (" + string.Join(" AND ", notLikeConditions) + ")");
            return this;
        }

        public SqlQueryBuilder AndGroup(Action<SqlQueryBuilder> groupBuilder)
        {
            if (_where.Count > 0)
                _where.Add("AND");

            _where.Add("(");
            var nested = new SqlQueryBuilder(this);
            groupBuilder(nested);
            _where.AddRange(nested._where);
            _where.Add(")");

            foreach (var kv in nested._parameters)
                _parameters[kv.Key] = kv.Value;

            return this;
        }

        public SqlQueryBuilder OrGroup(Action<SqlQueryBuilder> groupBuilder)
        {
            if (_where.Count > 0)
                _where.Add("OR");

            _where.Add("(");
            var nested = new SqlQueryBuilder(this);
            groupBuilder(nested);
            _where.AddRange(nested._where);
            _where.Add(")");

            foreach (var kv in nested._parameters)
                _parameters[kv.Key] = kv.Value;

            return this;
        }

        public SqlQueryBuilder GroupBy(string group)
        {
            _groupBy = group;
            return this;
        }

        public SqlQueryBuilder Having(string condition)
        {
            _having.Add(condition);
            return this;
        }

        public SqlQueryBuilder Having(string condition, object value)
        {
            string paramName = $"@p{_paramCounter++}";
            _having.Add(condition.Replace("?", paramName));
            _parameters[paramName] = value;
            return this;
        }

        public SqlQueryBuilder HavingGroup(Action<SqlQueryBuilder> groupBuilder, string logic = "AND")
        {
            if (_having.Count > 0)
                _having.Add(logic);

            _having.Add("(");
            var nested = new SqlQueryBuilder(this);
            groupBuilder(nested);
            _having.AddRange(nested._having);
            _having.Add(")");

            foreach (var kv in nested._parameters)
                _parameters[kv.Key] = kv.Value;

            return this;
        }

        public SqlQueryBuilder OrHavingGroup(Action<SqlQueryBuilder> groupBuilder)
        {
            return HavingGroup(groupBuilder, "OR");
        }

        public SqlQueryBuilder OrderBy(string order)
        {
            _orderBy = order;
            return this;
        }

        public SqlQueryBuilder Limit(int count)
        {
            _limit = count;
            return this;
        }

        public SqlQueryBuilder Offset(int count)
        {
            _offset = count;
            return this;
        }

        public SqlQueryBuilder Union(string sql)
        {
            _unions.Add(sql);
            return this;
        }

        public SqlQueryBuilder UnionAll(string sql)
        {
            _unions.Add("ALL " + sql);
            return this;
        }

        public SqlQueryBuilder AddParameter(string name, object value)
        {
            _parameters[name] = value;
            return this;
        }

        public (string Sql, Dictionary<string, object> Parameters) Build()
        {
            if (_from == string.Empty)
                throw new InvalidOperationException("FROM clause is required.");

            string sql = "";
            if (_withClauses.Any())
                sql += "WITH " + string.Join(", ", _withClauses) + " ";

            sql += $"SELECT {string.Join(", ", _select)} FROM {_from}";

            if (_joins.Any())
                sql += " " + string.Join(" ", _joins);

            if (_where.Any())
                sql += " WHERE " + string.Join(" ", _where);

            if (!string.IsNullOrEmpty(_groupBy))
                sql += " GROUP BY " + _groupBy;

            if (_having.Any())
                sql += " HAVING " + string.Join(" ", _having);

            if (!string.IsNullOrEmpty(_orderBy))
                sql += " ORDER BY " + _orderBy;

            if (_limit.HasValue)
                sql += $" LIMIT {_limit.Value}";

            if (_offset.HasValue)
                sql += $" OFFSET {_offset.Value}";

            if (_unions.Any())
                sql = "(" + sql + ") UNION " + string.Join(" UNION ", _unions.Select(u => "(" + u + ")"));

            return (sql, _parameters);
        }
    }

}
