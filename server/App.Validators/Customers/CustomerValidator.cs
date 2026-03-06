using App.Core.Domain.Customers;
using App.Data;
using App.Models.Customers;
using App.Services.Customers;
using App.Services.Localization;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Validators
{
    public partial class CustomerValidator : BaseNopValidator<CustomerModel>
    {
        public CustomerValidator(CustomerSettings customerSettings,
            ICustomerService customerService,
            ILocalizationService localizationService,
            INopDataProvider dataProvider)
        {
            //ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                //.WithMessage("Valid Email is required for customer to be in 'Registered' role")
                .WithMessageAwait(localizationService.GetResourceAsync("App.Errors.WrongEmail"))
                //only for registered users
                .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));

            RuleFor(x => x.NickName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.CustomerModel.Validation.NickName"));
            RuleFor(x => x.SystemName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.CustomerModel.Validation.SystemName"));
            RuleFor(x => x.SelectedCustomerRoleIds).Must((x, context) =>
            {
                var isInRole = x.SelectedCustomerRoleIds.Count > 0;
                return isInRole;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.CustomerModel.Validation.SelectedCustomerRoleIds"));

        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<bool> IsRegisteredCustomerRoleCheckedAsync(CustomerModel model, ICustomerService customerService)
        {
            var allCustomerRoles = await customerService.GetAllCustomerRolesAsync(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            var isInRegisteredRole = newCustomerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName) != null;
            return isInRegisteredRole;
        }
    }
}