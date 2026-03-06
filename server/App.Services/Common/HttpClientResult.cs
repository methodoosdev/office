using System.Collections.Generic;
using System.Linq;

namespace App.Services.Common
{
    public class HttpClientResult
    {
        private readonly IList<string> _errors;

        public HttpClientResult()
        {
            _errors = new List<string>();
        }

        public string Content { get; set; }

        public bool Success => !_errors.Any();

        public void AddError(string error)
        {
            _errors.Add(error);
        }

        public string Error => _errors.FirstOrDefault();
    }
}