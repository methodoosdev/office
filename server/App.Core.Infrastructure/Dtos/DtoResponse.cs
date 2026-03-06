using System.Collections.Generic;
using System.Linq;

namespace App.Core.Infrastructure.Dtos
{
    public partial class DtoResponse<T> where T : class
    {
        public DtoResponse()
        {
            Messages = new List<string>();
            Errors = new List<string>();
        }

        public bool Success => !Errors.Any();
        public string Message => Messages.FirstOrDefault();
        public string Error => Errors.FirstOrDefault();

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public List<string> Messages { get; set; }
        public List<string> Errors { get; set; }
        public T Model { get; set; }
    }
}