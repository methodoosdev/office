using App.Core;
using App.Core.Infrastructure;
using App.Data;
using App.Models.Messages;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class EmailMessageValidator : BaseNopValidator<EmailMessageModel>
    {
        public EmailMessageValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.TraderId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.EmailMessageModel.Validation.TraderId"));
            RuleFor(x => x.ToAddress).Must((x, context) =>
            {
                return CommonHelper.IsValidEmail(x.ToAddress);
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.EmailMessageModel.Validation.ToAddress"));

            RuleFor(x => x.ToName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.EmailMessageModel.Validation.ToName"));
        }
    }
}