using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record CountingDocumentSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime Periodos {  get; set; }
    }
    public partial record CountingDocumentListModel : BasePagedListModel<CountingDocumentModel>
    {
    }
    public partial record CountingDocumentModel : BaseNopEntityModel
    {
        public int RecCount { get; set; }
        public string DocName { get; set; }
        public string DocType { get; set; }
    }
    public partial record CountingDocumentTableModel : BaseNopModel
    {
    }
}