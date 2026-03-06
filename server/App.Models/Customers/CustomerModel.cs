using App.Core;
using App.Core.Domain.Common;
using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Customers
{
    public partial record CustomerDialogModel : BaseNopModel
    {
        public int ParentId { get; set; }
    }
    public partial record CustomerDialogFormModel : BaseNopModel
    {
    }
    public partial record CustomerSearchModel : BaseSearchModel
    {
        public CustomerSearchModel() : base("email")
        {
            SelectedCustomerRoleIds = new List<int>();
        }
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public string SearchEmail { get; set; }
        public string SearchUsername { get; set; }
        public string SearchIpAddress { get; set; }
    }
    public partial record CustomerListModel : BasePagedListModel<CustomerModel>
    {
    }
    public partial record CustomerModelHelper
    {
        public CustomerModelHelper()
        {
            AvailableTimeZones = new List<SelectionList>();
            AvailableCountries = new List<SelectionItemList>();
            AvailableEmployees = new List<SelectionItemList>();
            AvailableTraders = new List<SelectionItemList>();
            AvailableCustomerRoles = new List<SelectionItemList>();
        }

        public IList<SelectionItemList> AvailableEmployees { get; set; }
        public IList<SelectionItemList> AvailableTraders { get; set; }
        public IList<SelectionItemList> AvailableCountries { get; set; }
        public IList<SelectionList> AvailableTimeZones { get; set; }
        public IList<SelectionItemList> AvailableCustomerRoles { get; set; }
    }
    public partial record CustomerModel : BaseNopEntityModel, IFullName
    {
        public CustomerModel()
        {
            SendEmail = new SendEmailModel() { SendImmediately = true };
            SendPm = new SendPmModel();

            SelectedCustomerRoleIds = new List<int>();
            SelectedNewsletterSubscriptionStoreIds = new List<int>();
            //CustomerActivityLogSearchModel = new CustomerActivityLogSearchModel();
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string NickName { get; set; }
        public string SystemName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int TraderId { get; set; }
        public string TraderName { get; set; }
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public bool Active { get; set; }
        public string AdminComment { get; set; }
        public string TimeZoneId { get; set; }
        public string LastIpAddress { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public string RegisteredInStore { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int CountryId { get; set; }
        public string CustomerRoleNames { get; set; }
        public IList<int> SelectedNewsletterSubscriptionStoreIds { get; set; }
        public SendEmailModel SendEmail { get; set; }
        public SendPmModel SendPm { get; set; }
        public bool AllowSendingOfPrivateMessage { get; set; }
        public bool AllowSendingOfWelcomeMessage { get; set; }
        public bool AllowReSendingOfActivationMessage { get; set; }
        public string AvatarUrl { get; set; }
        //public CustomerActivityLogSearchModel CustomerActivityLogSearchModel { get; set; }
        public string Username { get; set; }
        public int NumPosts { get; set; }
    }
    public partial record CustomerFormModel : BaseNopModel
    {
    }

    public partial record SendEmailModel : BaseNopModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool SendImmediately { get; set; }
        public DateTime? DontSendBeforeDate { get; set; }
    }

    public partial record SendPmModel : BaseNopModel
    {
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}