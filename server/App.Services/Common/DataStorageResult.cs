using System.Collections.Generic;
using System.Linq;

namespace App.Services.Common
{
    public class DataStorageResult<T, U>
    {
        private List<T> dataList;
        private U singleData;

        public DataStorageResult()
        {
            dataList = new List<T>();
            Errors = new List<string>();
        }

        public void AddToList(T data)
        {
            dataList.Add(data);
        }

        public void AddToList(IList<T> items)
        {
            dataList.AddRange(items);
        }

        public List<T> GetDataList()
        {
            return dataList;
        }

        public void SetSingleData(U data)
        {
            singleData = data;
        }

        public U GetSingleData()
        {
            return singleData;
        }
        public bool Success => !Errors.Any();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public IList<string> Errors { get; set; }
    }
}