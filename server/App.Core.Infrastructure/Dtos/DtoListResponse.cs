using System.Collections.Generic;
using System.Linq;

namespace App.Core.Infrastructure.Dtos
{
    public partial class DtoListResponse<T> where T : class
    {
        public DtoListResponse()
        {
            Messages = new List<string>();
            Errors = new List<string>();
            List = new List<T>();
        }

        public bool Success => !Errors.Any();
        public string Message => Messages.FirstOrDefault();
        public string Error => Errors.FirstOrDefault();

        public void AddItem(T item)
        {
            List.Add(item);
        }

        public void AddRange(IList<T> list)
        {
            List.AddRange(list);
        }

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
        public List<T> List { get; set; }
    }
}