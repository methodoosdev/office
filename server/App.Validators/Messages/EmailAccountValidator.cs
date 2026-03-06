using App.Core.Domain.Messages;
using App.Data;
using App.Models.Messages;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class EmailAccountValidator : BaseNopValidator<EmailAccountModel>
    {
        public EmailAccountValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.EmailAccountModel.Validation.Email"));
            RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("App.Errors.WrongEmail"));

            RuleFor(x => x.DisplayName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.EmailAccountModel.Validation.DisplayName"));

            SetDatabaseValidationRules<EmailAccount>(dataProvider);
        }
    }
}