using App.Core.Domain.Employees;
using App.Core.Domain.Messages;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Models.Payroll;
using App.Services.Common;
using App.Services.Employees;
using App.Services.Logging;
using App.Services.Messages;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Payroll
{
    public partial interface IPersonTermExpiryService
    {
        Task Check();
    }

    public partial class PersonTermExpiryService : IPersonTermExpiryService
    {
        private readonly ITraderService _traderService;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ISqlConnectionService _connectionService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILogger _logger;

        public PersonTermExpiryService(
            ITraderService traderService,
            ITraderEmployeeMappingService traderEmployeeMappingService,
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IQueuedEmailService queuedEmailService,
            ISqlConnectionService connectionService,
            IAppDataProvider dataProvider,
            ILogger logger)
        {
            _traderService = traderService;
            _traderEmployeeMappingService = traderEmployeeMappingService;
            _employeeService = employeeService;
            _departmentService = departmentService;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _queuedEmailService = queuedEmailService;
            _connectionService = connectionService;
            _dataProvider = dataProvider;
            _logger = logger;
        }

        public virtual async Task Check()
        {
            await SendEmailsAsync();
        }

        private async Task _SendEmailsAsync()
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return;

            var people = await _dataProvider.QueryAsync<PersonTermExpiryModel>(result.Connection, PayrollDefaults.PersonTermExpiryQuery);
            people = people.AsEnumerable().Where(x => x.DateStatusId > 0).ToList();
            //var companyIds = people.Select(x => x.CompanyId).Distinct().ToList();

            var employeeList = await _employeeService.GetEmployeesByDepartmentSystemNameAsync(DepartmentDefaults.PayrollName);
            var employees = employeeList.Where(x => x.PayrollInfoEmail && CommonHelper.IsValidEmail(x.EmailContact)).ToList();

            foreach (var employee in employees)
            {
                var traders = await _traderEmployeeMappingService.GetTradersByEmployeeIdAsync(employee.Id);
                foreach (var trader in traders)
                {
                    var list = people.Where(x => x.CompanyId == trader.HyperPayrollId || x.Vat == trader.Vat).ToList();
                    list.ForEach(x => x.Email = employee.EmailContact);
                    //companyIds.Remove(trader.HyperPayrollId);
                }
            }
            var defaultEmailAccountId = _emailAccountSettings.DefaultEmailAccountId;
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(defaultEmailAccountId);

            var subject = "Λήξη σύμβασης εκπροσώπων";

            var groups = people.GroupBy(k => k.Email).ToList();

            groups.ForEach(async item => {
                var email = item.Key;

                var body = GenerateHtmlTable(item.ToList());

                var queuedEmail = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    //ToName = customer.NickName,
                    To = email,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _queuedEmailService.InsertQueuedEmailAsync(queuedEmail);
            });
        }

        private async Task SendEmailsAsync()
        {
            var department = await _departmentService.GetDepartmentBySystemNameAsync(DepartmentDefaults.PayrollName);
            if (department == null)
                return;

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return;

            var persons = await _dataProvider.QueryAsync<PersonTermExpiryModel>(result.Connection, PayrollDefaults.PersonTermExpiryQuery);
            persons = persons.AsEnumerable().Where(x => x.DateStatusId > 0).ToList();

            foreach (var person in persons)
            {
                var trader = await _traderService.GetTraderByHyperPayrollIdAsync(person.CompanyId);
                if (trader == null)
                    trader = await _traderService.GetTraderByVatAsync(person.Vat);

                if (trader != null)
                {
                    var employees = await _traderEmployeeMappingService.GetEmployeesByTraderIdAsync(trader.Id);
                    var employee = employees.FirstOrDefault(x => x.PayrollInfoEmail && x.DepartmentId == department.Id);
                    if (employee == null)
                        continue;

                    var validEmail = CommonHelper.IsValidEmail(employee.EmailContact.Trim());
                    if (!validEmail)
                        continue;

                    person.Email = employee.EmailContact.Trim();
                }
            }

            var defaultEmailAccountId = _emailAccountSettings.DefaultEmailAccountId;
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(defaultEmailAccountId);

            var subject = "Λήξη σύμβασης εκπροσώπων";

            var groups = persons.GroupBy(k => k.Email).ToList();

            foreach (var group in groups)
            {
                var email = group.Key;
                if (!CommonHelper.IsValidEmail(email))
                {
                    var _employeeList = await _employeeService.GetEmployeesByDepartmentSystemNameAsync(DepartmentDefaults.PayrollName);
                    var supervisorId = _employeeList.FirstOrDefault(x => x.SupervisorId > 0)?.SupervisorId;
                    if (supervisorId.HasValue)
                    {
                        var superVisor = await _employeeService.GetEmployeeByIdAsync(supervisorId.Value);
                        var validEmail = CommonHelper.IsValidEmail(superVisor.EmailContact.Trim());
                        if (validEmail)
                            email = superVisor.EmailContact.Trim();
                        else
                            continue;
                    }
                }

                var body = GenerateHtmlTable(group.ToList());

                var queuedEmail = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    //ToName = customer.NickName,
                    To = email,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _queuedEmailService.InsertQueuedEmailAsync(queuedEmail);
            }
        }
        private string GenerateHtmlTable(IList<PersonTermExpiryModel> items)
        {
            var sb = new StringBuilder();

            // Add CSS styles
            sb.AppendLine("<style>");
            sb.AppendLine(@"
                .styled-table {
                    width: 100%;
                    border-collapse: collapse;
                    margin: 25px 0;
                    font-size: 16px;
                    text-align: left;
                }
                .styled-table th, .styled-table td {
                    padding: 12px 15px;
                }
                .styled-table thead tr {
                    background-color: aliceblue;
                    text-align: left;
                    font-weight: bold;
                }
                .styled-table tbody tr {
                    border-bottom: 1px solid #dddddd;
                }
                .styled-table tbody tr:nth-of-type(even) {
                    background-color: #f3f3f3;
                }
                .styled-table tbody tr:last-of-type {
                    border-bottom: 2px solid aliceblue;
                }
            ");
            sb.AppendLine("</style>");

            // Start table
            sb.AppendLine("<table class=\"styled-table\">");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th>Επωνυμία</th>");
            sb.AppendLine("<th>Υποκατάστημα</th>");
            sb.AppendLine("<th>Εκπρόσωπος</th>");
            sb.AppendLine("<th>Είδος εκπ/που</th>");
            sb.AppendLine("<th>Ημ.Λήξης</th>");
            sb.AppendLine("<th>Έλεγχος σύμβασης</th>");
            sb.AppendLine("<th>Έλεγχος σύμβασης σε ημέρες</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            // Add table rows
            foreach (var item in items)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{item.CompanyName}</td>");
                sb.AppendLine($"<td>{item.BranchName}</td>");
                sb.AppendLine($"<td>{item.PersonName}</td>");
                sb.AppendLine($"<td>{item.PersonType}</td>");
                sb.AppendLine($"<td>{item.ExpirationDate.ToString("dd/MM/yyyy")}</td>");
                sb.AppendLine($"<td>{item.DateStatus}</td>");
                sb.AppendLine($"<td>{item.DayStatus}</td>");
                sb.AppendLine("</tr>");
            }

            // End table
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }
    }
}