using App.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace App.Models.Logging
{
    public partial record LogFilterModel : BaseNopModel
    {
        public LogFilterModel()
        {
            AvailableLogLevels = new List<SelectListItem>();
        }

        public DateTime? CreatedOnFrom { get; set; }
        public DateTime? CreatedOnTo { get; set; }
        public string Message { get; set; }
        public int LogLevelId { get; set; }

        public IList<SelectListItem> AvailableLogLevels { get; set; }
    }
    public partial record LogFilterFormModel : BaseNopModel
    {
    }
    public partial record LogSearchModel : BaseSearchModel
    {
        public LogSearchModel() : base("shortMessage") { }
    }
    public partial record LogModel : BaseNopEntityModel
    {
        public string LogLevel { get; set; }
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }
        public string IpAddress { get; set; }
        public int? CustomerId { get; set; }
        public string PageUrl { get; set; }
        public string ReferrerUrl { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CustomerEmail { get; set; }
        public string LogLevelName { get; set; }
    }
    public partial record LogFormModel : BaseNopModel
    {
    }
    public partial record LogListModel : BasePagedListModel<LogModel>
    {
    }
}