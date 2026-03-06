using App.Core.Domain.Common;
using System;

namespace App.Core.Domain.Customers
{
    public partial class Customer : BaseEntity, IFullName, ISoftDeletedEntity
    {
        public Customer()
        {
            CustomerGuid = Guid.NewGuid();
        }

        public Guid CustomerGuid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Company { get; set; }
        public string StreetAddress { get; set; }
        public string StreetAddress2 { get; set; }
        public string ZipPostalCode { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public int? CountryId { get; set; }
        public string Phone { get; set; }
        public string VatNumber { get; set; }
        public string TimeZoneId { get; set; }
        public int? CurrencyId { get; set; }
        public int? LanguageId { get; set; }
        public string EmailToRevalidate { get; set; }
        public string AdminComment { get; set; }
        public bool RequireReLogin { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? CannotLoginUntilDateUtc { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool IsSystemAccount { get; set; }
        public string SystemName { get; set; }
        public string LastIpAddress { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? LastLoginDateUtc { get; set; }
        public DateTime LastActivityDateUtc { get; set; }
        public int RegisteredInStoreId { get; set; }
        public int NumPosts { get; set; }

        //custom
        public string NickName { get; set; }
        public int EmployeeId { get; set; }
        public int TraderId { get; set; }






        public int VendorId { get; set; }
        public int AvatarPictureId { get; set; }
        public string LastVisitedPage { get; set; }
        public string AccountActivationToken { get; set; }
        public string EmailRevalidationToken { get; set; }
        public string PasswordRecoveryToken { get; set; }
        public DateTime? PasswordRecoveryTokenDateGenerated { get; set; }
        public bool LanguageAutomaticallyDetected { get; set; }
        public int PageSize { get; set; }
        public string PageSizeOptions { get; set; }
    }
}