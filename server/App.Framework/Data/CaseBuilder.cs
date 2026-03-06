using System.Collections.Generic;
using System.Linq;

namespace App.Framework.Data
{
    public class CaseBuilder
    {
        private class WhenThenClause
        {
            public string Expression { get; set; } = "";
            public string ThenResult { get; set; }
        }

        private readonly List<WhenThenClause> _clauses = new();
        private string _elseValue;
        private string _alias;
        private readonly Dictionary<string, object> _parameters = new();
        private int _paramCounter = 0;
        private WhenThenClause _currentClause;

        public CaseBuilder When(string column, string op, object value)
        {
            string paramName = $"@case{_paramCounter++}";
            _parameters[paramName] = value;

            string condition = $"{column} {op} {paramName}";
            _currentClause = new WhenThenClause { Expression = condition };
            _clauses.Add(_currentClause);
            return this;
        }

        public CaseBuilder WhenLike(string column, string pattern)
        {
            string paramName = $"@case{_paramCounter++}";
            _parameters[paramName] = pattern;

            string condition = $"{column} LIKE {paramName}";
            _clauses.Add(new WhenThenClause { Expression = condition });
            return this;
        }

        public CaseBuilder WhenIn(string column, IEnumerable<object> values, string thenResult)
        {
            var paramNames = new List<string>();
            foreach (var value in values)
            {
                string paramName = $"@case{_paramCounter++}";
                _parameters[paramName] = value;
                paramNames.Add(paramName);
            }

            string condition = $"{column} IN ({string.Join(", ", paramNames)})";
            _clauses.Add(new WhenThenClause { Expression = condition, ThenResult = thenResult });
            return this;
        }

        public CaseBuilder Then(string result)
        {
            if (_currentClause != null)
                _currentClause.ThenResult = result;
            return this;
        }

        public CaseBuilder Else(string result)
        {
            _elseValue = result;
            return this;
        }

        public CaseBuilder As(string alias)
        {
            _alias = alias;
            return this;
        }

        public override string ToString()
        {
            var whenParts = _clauses.Select(c =>
                $"WHEN {c.Expression} THEN {c.ThenResult ?? "NULL"}");

            var result = $"CASE {string.Join(" ", whenParts)}";

            if (_elseValue != null)
                result += $" ELSE {_elseValue}";

            result += " END";

            if (!string.IsNullOrEmpty(_alias))
                result += $" AS {_alias}";

            return result;
        }

        public Dictionary<string, object> GetParameters() => _parameters;
    }

}
