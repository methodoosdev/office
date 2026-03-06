using App.Data;
using App.Models.Traders;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class AccountingWorkValidator : BaseNopValidator<AccountingWorkModel>
    {
        public AccountingWorkValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.SortDescription).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.AccountingWorkModel.Validation.SortDescription"));            
        }
    }
}