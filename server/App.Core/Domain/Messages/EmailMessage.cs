using System;

namespace App.Core.Domain.Messages
{
    public partial class EmailMessage : BaseEntity
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

        //migration
        public int SenderId { get; set; }
        public int TraderId { get; set; }
        public int Period { get; set; }
    }
}
