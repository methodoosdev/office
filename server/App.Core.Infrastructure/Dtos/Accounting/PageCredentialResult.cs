namespace App.Core.Infrastructure.Dtos.Accounting
{
    public enum PageCredentialType
    {
        IndividualCompany = 1,
        Accountant = 2,
        Representative = 4
    }
    public class PageCredentialResult
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int PageCredentialTypeId { get; set; }
        public PageCredentialType PageCredentialType
        {
            get => (PageCredentialType)PageCredentialTypeId;
            set => PageCredentialTypeId = (int)value;
        }

    }
}
