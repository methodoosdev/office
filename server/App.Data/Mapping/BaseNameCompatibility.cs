using App.Core.Domain.Customers;
using App.Core.Domain.Forums;
using App.Core.Domain.News;
using App.Core.Domain.Security;
using System;
using System.Collections.Generic;

namespace App.Data.Mapping
{
    /// <summary>
    /// Base instance of backward compatibility of table naming
    /// </summary>
    public partial class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new()
        {
        };

        public Dictionary<(Type, string), string> ColumnName => new()
        {
        };
    }
}