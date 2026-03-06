using App.Core.Domain.Messages;
using App.Data.Mapping;
using App.Models.Messages;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class QueuedEmailValidator : BaseNopValidator<QueuedEmailModel>
    {
        public QueuedEmailValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            //RuleFor(x => x.From).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.From.Required"));
            RuleFor(x => x.To).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.QueuedEmailModel.Validation.To"));
            RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.QueuedEmailModel.Validation.Subject"));

            //RuleFor(x => x.SentTries).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.SentTries.Required"))
            //                        .InclusiveBetween(0, 99999).WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.SentTries.Range"));

            //SetDatabaseValidationRules<QueuedEmail>(mappingEntityAccessor);

        }
    }
}