using App.Data;
using App.Models.Accounting;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class MyDataItemValidator : BaseNopValidator<MyDataItemModel>
    {
        public MyDataItemValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Series).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.MyDataItemModel.Validation.Series"));
            RuleFor(x => x.CounterpartVat).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.MyDataItemModel.Validation.CounterpartVat"));
            RuleFor(x => x.InvoiceType).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.MyDataItemModel.Validation.InvoiceType"));
            RuleFor(x => x.PaymentMethodId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.MyDataItemModel.Validation.PaymentMethodId"));
            RuleFor(x => x.SeriesId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.MyDataItemModel.Validation.SeriesId"));
            RuleFor(x => x.ProductCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.MyDataItemModel.Validation.ProductCode"));

            RuleFor(x => x.VatCategoryId).Must((x, context) =>
            {
                return x.VatCategoryId > 0 || x.TaxCategoryId > 0;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.MyDataItemModel.Validation.CategoryId"));

            RuleFor(x => x.TaxCategoryId).Must((x, context) =>
            {
                return x.VatCategoryId > 0 || x.TaxCategoryId > 0;
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.MyDataItemModel.Validation.CategoryId"));
        }
    }
}