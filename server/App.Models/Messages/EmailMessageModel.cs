using App.Framework.Models;
using System;

namespace App.Models.Messages
{
    public partial record EmailMessageFilterModel : BaseNopModel
    {
        public int EmailMessageTypeId { get; set; } = 0;
        public string Description { get; set; } = null;
        public DateTime? ShippingDate { get; set; } = null;
        public string Subject { get; set; } = null;
        public string ToAddress { get; set; } = null;
        public string ToName { get; set; } = null;
        public int CustomerId { get; set; } = 0;

        public int IsSent { get; set; } = 0;
        public int? CustomerTypeId { get; set; } = -1;
        public int? LegalFormTypeId { get; set; } = -1;
        public int? CategoryBookTypeId { get; set; } = -1;
        public string CustomerName { get; set; } = null;
        public DateTime? PeriodDate { get; set; } = null;


    }
    public partial record EmailMessageFilterFormModel : BaseNopModel
    {
    }
    public partial record EmailMessageSearchModel : BaseSearchModel
    {
        public EmailMessageSearchModel() : base("createdDate", "desc") { }
    }
    public partial record EmailMessageListModel : BasePagedListModel<EmailMessageModel>
    {
    }
    public partial record EmailMessageModel : BaseNopEntityModel
    {
        public int CustomerId { get; set; }
        public int EmailMessageTypeId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string ToAddress { get; set; }
        public string ToName { get; set; }
        public string ReplyTo { get; set; }
        public string ReplyToName { get; set; }
        public string AttachmentFilePath { get; set; }
        public string AttachmentFileName { get; set; }
        public int AttachedDownloadId { get; set; }
        public string Headers { get; set; }
        public string Bcc { get; set; }
        public string Cc { get; set; }
        public int SenderId { get; set; }
        public int TraderId { get; set; }
        public int Period { get; set; }

        public bool IsSent { get; set; }
        public string EmailMessageTypeName { get; set; } = null;
        public string TraderName { get; set; }
        public int CustomerTypeId { get; set; }
        public string CustomerTypeName { get; set; }
        public int LegalFormTypeId { get; set; }
        public string LegalFormTypeName { get; set; }
        public int CategoryBookTypeId { get; set; }
        public string CategoryBookTypeName { get; set; }
        public string PeriodName { get; set; }
        public string SenderName { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public partial record EmailMessageFormModel : BaseNopModel
    {
    }
}