using System.Collections.Generic;
using System.Linq;

namespace App.Automation
{
    public partial class PageErrorResult
    {
        public PageErrorResult()
        {
            Errors = new Dictionary<string, string>();
        }

        public bool Success => !Errors.Any();

        public void AddError(string error)
        {
            Errors.Add(error, string.Empty);
        }

        public void AddError(string error, string param)
        {
            Errors.Add(error, param);
        }

        public Dictionary<string, string> Errors { get; set; }
    }
}