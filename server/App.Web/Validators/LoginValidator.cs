using App.Core.Domain.Customers;
using App.Services.Localization;
using App.Validators;
using App.Web.Models;
using FluentValidation;

namespace App.Web.Validators
{
    public partial class LoginValidator : BaseNopValidator<LoginModel>
    {
        public LoginValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            //if (!customerSettings.UsernamesEnabled)
            //{
            //    //login by email
            //    RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Errors.EmailRequired"));
            //    RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("App.Errors.WrongEmail"));
            //}
        }
    }
}