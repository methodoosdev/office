using App.Framework.Models;
using System;

namespace App.Models.Messages
{
    public partial record QueuedEmailSearchModel : BaseSearchModel
    {
        public QueuedEmailSearchModel() : base("email") { }
        public DateTime? SearchStartDate { get; set; }
        public DateTime? SearchEndDate { get; set; }
    }
    public partial record QueuedEmailListModel : BasePagedListModel<QueuedEmailModel>
    {
    }
    public partial record QueuedEmailModel: BaseNopEntityModel
    {
        public int PriorityId { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string To { get; set; }
        public string ToName { get; set; }
        public string ReplyTo { get; set; }
        public string ReplyToName { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AttachmentFilePath { get; set; }
        public string AttachmentFileName { get; set; }
        public int AttachedDownloadId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? DontSendBeforeDateUtc { get; set; }
        public int SentTries { get; set; }
        public DateTime? SentOnUtc { get; set; }
        public int EmailAccountId { get; set; }

        public DateTime? SentOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PriorityName { get; set; }
        public string EmailAccountName { get; set; }
    }

    public partial record QueuedEmailFormModel : BaseNopModel
    {
    }
}