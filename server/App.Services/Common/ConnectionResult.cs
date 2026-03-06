using System.Collections.Generic;
using System.Linq;

namespace App.Services.Common
{
    public class ConnectionResult
    {
        private readonly IList<string> _errors;

        public ConnectionResult()
        {
            _errors = new List<string>();
        }

        public string Connection { get; set; }

        public bool Success => !_errors.Any();

        public void AddError(string error)
        {
            _errors.Add(error);
        }

        public string Error => _errors.FirstOrDefault();
    }
}