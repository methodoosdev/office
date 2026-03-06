using App.Models.Scripts;
using App.Services.Localization;
using App.Services.Scripts;
using FluentValidation;
using System.Text.RegularExpressions;

namespace App.Validators
{
    public partial class ScriptValidator : BaseNopValidator<ScriptModel>
    {
        private readonly Regex TokenRegex =
            new(@"^#[A-Za-z\u0370-\u03FF\u1F00-\u1FFF\p{M}\p{Nd}_\.\-\(\)\[\]/\?<>:]+$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public ScriptValidator(ILocalizationService localizationService, IScriptService scriptService)
        {
            RuleFor(x => x.ScriptName).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.ScriptModel.Validation.ScriptName"));

            RuleFor(x => x.Replacement).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.ScriptModel.Validation.Replacement.NotEmpty"));

            RuleFor(x => x.Replacement).Must((x, context) =>
            {
                if (string.IsNullOrEmpty(x.Replacement))
                    return true;

                var match = TokenRegex.IsMatch(x.Replacement);

                return match;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.ScriptModel.Validation.Replacement.TokenRegex"));

            RuleFor(x => x.Replacement).MustAwait(async (x, context) =>
            {
                var entity = await scriptService.GetScriptByReplacementAsync(x.TraderId, x.Id, x.Replacement);

                return entity == null ? true : false;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.ScriptModel.Validation.Replacement.Unique"));

        }
    }
}