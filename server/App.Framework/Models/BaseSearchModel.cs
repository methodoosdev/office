using App.Core.Domain.Common;
using App.Core.Infrastructure;
using App.Framework.Components;
using System.Collections.Generic;

namespace App.Framework.Models
{
    /// <summary>
    /// Represents base search model
    /// </summary>
    public abstract partial record BaseSearchModel : BaseNopModel, IPagingRequestModel
    {
        protected BaseSearchModel(string sortField, string sortOrder = "asc")
        {
            //State = new GridState(sortField, sortOrder);
            Length = 15;
            SortField = sortField;
            SortOrder = sortOrder;
            Group = new();
        }

        /// <summary>
        /// Gets a page number
        /// </summary>
        public int Page => (Start / Length) + 1;

        /// <summary>
        /// Gets a page size
        /// </summary>
        public int PageSize => Length;

        /// <summary>
        /// Gets or sets a comma-separated list of available page sizes
        /// </summary>
        public string AvailablePageSizes { get; set; }

        /// <summary>
        /// Gets or sets draw. Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence).
        /// </summary>
        public string Draw { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public string QuickSearch { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        
        public string Title { get; set; }
        public string DataKey { get; set; }
        public int? Height { get; set; }

        public PagerSettings PagerSettings { get; set; }
        public List<ColumnConfig> Columns { get; set; }
        public List<GridGroupDescriptor> Group { get; set; }

        /// <summary>
        /// Set grid page parameters
        /// </summary>
        public void SetGridPageSize()
        {
            var adminAreaSettings = EngineContext.Current.Resolve<AdminAreaSettings>();
            SetGridPageSize(adminAreaSettings.DefaultGridPageSize, adminAreaSettings.GridPageSizes);
        }

        /// <summary>
        /// Set popup grid page parameters
        /// </summary>
        public void SetGridPageSize(int pageSize)
        {
            var adminAreaSettings = EngineContext.Current.Resolve<AdminAreaSettings>();
            SetGridPageSize(pageSize, adminAreaSettings.GridPageSizes);
        }

        /// <summary>
        /// Set grid page parameters
        /// </summary>
        /// <param name="pageSize">Page size; pass null to use default value</param>
        /// <param name="availablePageSizes">Available page sizes; pass null to use default value</param>
        public void SetGridPageSize(int pageSize, string availablePageSizes = null)
        {
            Start = 0;
            Length = pageSize;
            AvailablePageSizes = availablePageSizes;
        }
    }
}