using App.Core;
using App.Core.Infrastructure;
using App.Models.Customers;
using App.Services.Customers;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Customers
{
    public partial class CustomerSecurityModelFactory : ICustomerSecurityModelFactory
    {
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerPermissionService _customerPermissionService;

        public CustomerSecurityModelFactory(ICustomerService customerService,
            ILocalizationService localizationService,
            ICustomerPermissionService customerPermissionService)
        {
            _customerService = customerService;
            _localizationService = localizationService;
            _customerPermissionService = customerPermissionService;
        }

        public virtual async Task<CustomerPermissionMappingModel> PrepareCustomerPermissionMappingModelAsync(CustomerPermissionMappingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var customers = await _customerService.GetRegisterCustomersAsync();
            model.AvailableCustomers = customers.Select(t => new CustomerSecurityModel
            {
                Id = t.Id,
                Name = t.Username,
                SystemName = t.SystemName
            }).ToList();

            foreach (var permission in await _customerPermissionService.GetAllCustomerPermissionsAsync())
            {
                model.AvailableCustomerPermissions.Add(new CustomerPermissionModel
                {
                    Name = await _localizationService.GetLocalizedCustomerPermissionNameAsync(permission),
                    SystemName = permission.SystemName.ToCamelCase()
                });

                foreach (var customer in customers)
                {
                    if (!model.Allowed.ContainsKey(permission.SystemName))
                        model.Allowed[permission.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[permission.SystemName][customer.Id] =
                        (await _customerPermissionService.GetMappingByCustomerPermissionIdAsync(permission.Id)).Any(mapping => mapping.CustomerId == customer.Id);
                }
            }

            return model;
        }
    }
}