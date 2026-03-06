using System.Collections.Generic;
using System.Linq;

namespace App.Services.Common
{
    public partial class AppErrorResult
    {
        public AppErrorResult()
        {
            Errors = new List<string>();
        }

        public bool Success => !Errors.Any();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public IList<string> Errors { get; set; }
    }
}