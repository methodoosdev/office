using App.Core.Domain.Messages;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Messages
{
    public partial interface IEmailMessageService
    {
        IQueryable<EmailMessage> Table { get; }
        Task<EmailMessage> GetEmailMessageByIdAsync(int emailMessageId);
        Task<IList<EmailMessage>> GetEmailMessagesByIdsAsync(int[] emailMessageIds);
        Task<IList<EmailMessage>> GetAllEmailMessagesAsync();
        Task DeleteEmailMessageAsync(EmailMessage emailMessage);
        Task DeleteEmailMessageAsync(IList<EmailMessage> emailMessages);
        Task InsertEmailMessageAsync(EmailMessage emailMessage);
        Task InsertEmailMessageAsync(IList<EmailMessage> emailMessages);
        Task UpdateEmailMessageAsync(EmailMessage emailMessage);
        Task UpdateEmailMessageAsync(IList<EmailMessage> emailMessages);
    }
    public partial class EmailMessageService : IEmailMessageService
    {
        private readonly IRepository<EmailMessage> _emailMessageRepository;

        public EmailMessageService(
            IRepository<EmailMessage> emailMessageRepository)
        {
            _emailMessageRepository = emailMessageRepository;
        }

        public virtual IQueryable<EmailMessage> Table => _emailMessageRepository.Table;

        public virtual async Task<EmailMessage> GetEmailMessageByIdAsync(int emailMessageId)
        {
            return await _emailMessageRepository.GetByIdAsync(emailMessageId);
        }

        public virtual async Task<IList<EmailMessage>> GetEmailMessagesByIdsAsync(int[] emailMessageIds)
        {
            return await _emailMessageRepository.GetByIdsAsync(emailMessageIds);
        }

        public virtual async Task<IList<EmailMessage>> GetAllEmailMessagesAsync()
        {
            var entities = await _emailMessageRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Id);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteEmailMessageAsync(EmailMessage emailMessage)
        {
            await _emailMessageRepository.DeleteAsync(emailMessage);
        }

        public virtual async Task DeleteEmailMessageAsync(IList<EmailMessage> emailMessages)
        {
            await _emailMessageRepository.DeleteAsync(emailMessages);
        }

        public virtual async Task InsertEmailMessageAsync(EmailMessage emailMessage)
        {
            await _emailMessageRepository.InsertAsync(emailMessage);
        }

        public virtual async Task InsertEmailMessageAsync(IList<EmailMessage> emailMessages)
        {
            await _emailMessageRepository.InsertAsync(emailMessages);
        }

        public virtual async Task UpdateEmailMessageAsync(EmailMessage emailMessage)
        {
            await _emailMessageRepository.UpdateAsync(emailMessage);
        }
        public virtual async Task UpdateEmailMessageAsync(IList<EmailMessage> emailMessages)
        {
            await _emailMessageRepository.UpdateAsync(emailMessages);
        }

    }
}