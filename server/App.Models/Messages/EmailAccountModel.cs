using App.Framework.Models;
using System.ComponentModel.DataAnnotations;

namespace App.Models.Messages
{
    public partial record EmailAccountSearchModel : BaseSearchModel
    {
        public EmailAccountSearchModel() : base("email") { }
        public string Email { get; set; }
        public string Username { get; set; }
    }
    public partial record EmailAccountListModel : BasePagedListModel<EmailAccountModel>
    {
    }
    public partial record EmailAccountModel : BaseNopEntityModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool EnableSsl { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public bool IsDefaultEmailAccount { get; set; }

        public string SendTestEmailTo { get; set; }

        public string InfoMessage { get; set; }
    }
    public partial record EmailAccountFormModel : BaseNopModel
    {
    }
}