using App.Framework.Models;
using System;

namespace App.Models.Tasks
{
    public partial record ScheduleTaskSearchModel : BaseSearchModel
    {
        public ScheduleTaskSearchModel() : base("name") { }
    }
    public partial record ScheduleTaskListModel : BasePagedListModel<ScheduleTaskModel>
    {
    }
    public partial record ScheduleTaskModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public int Seconds { get; set; }

        public bool Enabled { get; set; }

        public bool StopOnError { get; set; }

        public string LastStartUtc { get; set; }

        public string LastEndUtc { get; set; }

        public string LastSuccessUtc { get; set; }


        public string Type { get; set; }
        public DateTime? LastEnabledUtc { get; set; }
    }
    public partial record ScheduleTaskFormModel : BaseNopModel
    {
    }
}