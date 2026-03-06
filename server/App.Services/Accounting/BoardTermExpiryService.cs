using App.Core;
using App.Core.Domain.Employees;
using App.Core.Domain.Messages;
using App.Core.Infrastructure;
using App.Models.Payroll;
using App.Models.Traders;
using App.Services.Employees;
using App.Services.Logging;
using App.Services.Messages;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Accounting
{
    public partial interface IBoardTermExpiryService
    {
        Task Check();
    }

    public partial class BoardTermExpiryService : IBoardTermExpiryService
    {
        private readonly ITraderService _traderService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger _logger;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IQueuedEmailService _queuedEmailService;

        public BoardTermExpiryService(
            ITraderService traderService,
            IEmployeeService employeeService,
            ILogger logger,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IQueuedEmailService queuedEmailService)
        {
            _traderService = traderService;
            _employeeService = employeeService;
            _logger = logger;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _queuedEmailService = queuedEmailService;
        }

        public virtual async Task Check()
        {
            await SendEmailsAsync();
        }

        private async Task SendEmailsAsync()
        {
            var employees = await _employeeService.GetEmployeesByDepartmentSystemNameAsync(DepartmentDefaults.AdministrationName);

            var employee = employees.FirstOrDefault(x => x.PayrollInfoEmail && CommonHelper.IsValidEmail(x.EmailContact));

            if (employee == null)
            {
                _logger.Error("Cannot execute BoardTermExpiryService");
                return;
            }

            var currentDate = DateTime.UtcNow;
            var models = new List<BoardTermExpiryModel>();
            var traders = await _traderService.GetAllTradersAsync(FieldConfigType.WithCategoryBookC);

            foreach (var trader in traders)
            {
                DateTime expirationDate = trader.BoardMemberExpiryDate.HasValue ? trader.BoardMemberExpiryDate.Value : new DateTime(2023, 26, 6);
                if (!trader.BoardMemberExpiryDate.HasValue)
                {
                    trader.BoardMemberExpiryDate = expirationDate;
                    await _traderService.UpdateTraderAsync(trader);
                }

                int dateStatusId = 0;
                string dateStatus = "Έγκυρη";
                string dayStatus = "Έγκυρη";
                bool expired = false;

                if (expirationDate < currentDate)
                {
                    dateStatusId = 1;
                    dateStatus = "Έληξε";
                    int daysElapsed = (currentDate - expirationDate).Days;
                    dayStatus = $"Έληξε πριν ({daysElapsed} ημέρες)";
                    expired = true;
                }
                else if (expirationDate >= currentDate && expirationDate <= currentDate.AddMonths(2))
                {
                    dateStatusId = 2;
                    dateStatus = "Προς λήξη";
                    int daysRemaining = (expirationDate - currentDate).Days;
                    dayStatus = $"Προς λήξη μετά από ({daysRemaining} ημέρες)";
                    expired = true;
                }

                if (expired)
                {
                    var model = new BoardTermExpiryModel();

                    model.CompanyName = trader.ToTraderFullName();
                    model.Email = employee.EmailContact;
                    model.ExpirationDate = expirationDate;
                    model.DateStatusId = dateStatusId;
                    model.DateStatus = dateStatus;
                    model.DayStatus = dayStatus;

                    models.Add(model);
                }
            }

            var persons = models.Where(x => x.DateStatusId > 0).OrderBy(x => x.CompanyName).ToList();

            var defaultEmailAccountId = _emailAccountSettings.DefaultEmailAccountId;
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(defaultEmailAccountId);

            var subject = "Λήξη διοικητικού συμβουλίου";

            var groups = persons.GroupBy(k => k.Email).ToList();

            groups.ForEach(async item =>
            {
                var email = item.Key;

                var body = GenerateHtmlTable(item.OrderBy(x => x.DayStatus).ToList());

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
        private string GenerateHtmlTable(IList<BoardTermExpiryModel> items)
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
            sb.AppendLine("<colgroup><col><col style=\"width: 110px;\"><col style=\"width: 110px;\"><col style=\"width: 220px;\"></colgroup>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th>Επωνυμία</th>");
            sb.AppendLine("<th>Ημ.Λήξης</th>");
            sb.AppendLine("<th>Έλεγχος ΔΣ</th>");
            sb.AppendLine("<th>Έλεγχος ΔΣ σε ημέρες</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            // Add table rows
            foreach (var item in items)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{item.CompanyName}</td>");
                sb.AppendLine($"<td>{item.ExpirationDate.ToString("dd/MM/yyyy")}</td>");
                sb.AppendLine($"<td>{item.DateStatus}</td>");
                sb.AppendLine($"<td>{item.DayStatus}</td>");
                sb.AppendLine("</tr>");
            }

            // End table
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            //sb.AppendLine(string.Join(", ", tradern));

            return sb.ToString();
        }
    }
}