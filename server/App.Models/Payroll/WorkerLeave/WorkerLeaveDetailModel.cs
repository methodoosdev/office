using App.Framework.Models;

namespace App.Models.Payroll
{
    public partial record WorkerLeaveDetailSearchModel : BaseSearchModel
    {
        public WorkerLeaveDetailSearchModel() : base("traderName") { }
        //public int TraderId { get; set; }
    }
    public partial record WorkerLeaveDetailListModel : BasePagedListModel<WorkerLeaveDetailModel>
    {
    }
    public partial record WorkerLeaveDetailModel : BaseNopEntityModel
    {
        public int TraderId { get; set; }
        public int DaysLeft { get; set; }
        public int WorkerId { get; set; }
        public string WorkerName { get; set; }
        public int Year { get; set; }

        public string TraderName { get; set; }
    }
    public partial record WorkerLeaveDetailFormModel : BaseNopModel
    {
        public string WorkerName { get; set; }
    }
    public partial class WorkerPoco
    {
        public int WorkerId { get; set; }
        public int CompanyId { get; set; }
        public string WorkerName { get; set; }
    }
}
