using App.Core.Domain.Customers;
using App.Core.Domain.Payroll;
using App.Core;
using App.Services.Customers;
using App.Services.Employees;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace App.Services.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ConnectionHub: Hub
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerOnlineService _customerOnlineService;
        private readonly IWebHelper _webHelper;

        public ConnectionHub(IEmployeeService employeeService,
            ICustomerService customerService,
            ICustomerOnlineService customerOnlineService,
            IWebHelper webHelper)
        {
            _employeeService = employeeService;
            _customerService = customerService;
            _customerOnlineService = customerOnlineService;
            _webHelper = webHelper;
        }

        public override async Task OnConnectedAsync()
        {
            if (!string.IsNullOrEmpty(Context.UserIdentifier))
            {
                var customerId = Int32.Parse(Context.UserIdentifier);
                var systemName = Context.User.FindFirst(claim => claim.Type == "SystemName")?.Value;
                if (systemName.Equals(NopCustomerDefaults.TradersRoleName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var customerOnline = await _customerOnlineService.GetCustomerOnlineByCustomerIdAsync(customerId);
                    if (customerOnline != null)
                    {
                        customerOnline.Online = true;
                        customerOnline.LastLoginDateUtc = DateTime.UtcNow;
                        customerOnline.LastIpAddress = _webHelper.GetCurrentIpAddress();
                        customerOnline.Visits = customerOnline.Visits + 1;

                        await _customerOnlineService.UpdateCustomerOnlineAsync(customerOnline);
                    }
                    else
                    {
                        customerOnline = new CustomerOnline
                        {
                            CustomerId = customerId,
                            Email = Context.User.FindFirst(claim => claim.Type == ClaimTypes.Email).Value,
                            SystemName = systemName,
                            Online = true,
                            LastLoginDateUtc = DateTime.UtcNow,
                            LastIpAddress = _webHelper.GetCurrentIpAddress(),
                            Visits = 1
                        };

                        await _customerOnlineService.InsertCustomerOnlineAsync(customerOnline);
                    }
                }
                if (systemName.Equals(NopCustomerDefaults.EmployeesRoleName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var customer = await _customerService.GetCustomerByIdAsync(customerId);
                    var employee = await _employeeService.GetEmployeeByIdAsync(customer.EmployeeId);
                    if (employee != null && employee.PayrollInfoEmail)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, WorkerScheduleDefaults.PayrollGroupName);
                    }
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (!string.IsNullOrEmpty(Context.UserIdentifier))
            {
                var customerId = Int32.Parse(Context.UserIdentifier);
                var customerOnline = await _customerOnlineService.GetCustomerOnlineByCustomerIdAsync(customerId);
                if (customerOnline != null)
                {
                    customerOnline.Online = false;

                    await _customerOnlineService.UpdateCustomerOnlineAsync(customerOnline);
                }
            }

            var systemName = Context.User.FindFirst(claim => claim.Type == "SystemName")?.Value;
            if (systemName.Equals(NopCustomerDefaults.EmployeesRoleName, StringComparison.InvariantCultureIgnoreCase))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, WorkerScheduleDefaults.PayrollGroupName);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task PayrollSendToGroup(string message)
        {
            //await Clients.User(Context.UserIdentifier).SendAsync("ReceiveMessage", message);
            await Clients.Group(WorkerScheduleDefaults.PayrollGroupName).SendAsync("workerScheduleSignal", message);
        }
        public async Task SendMessage(string connectionId, string message)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }
    }
}
