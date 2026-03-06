using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using System.Collections.Generic;

namespace App.Models.Traders
{
    public static class TraderExtensions
    {
        //public static TraderDecryptModel ToTraderDecrypt(this Trader trader)
        //{
        //    return trader.ToModel<TraderDecryptModel>();
        //}
        public static string ToTraderFullName(this Trader trader)
        {
            return trader.ToModel<TraderFullNameModel>().FullName();
        }
        public static (string Vat, string FullName) ToTraderDecrypt(this Trader trader)
        {
            var decrypted = trader.ToModel<TraderFullNameModel>();
            return (Vat: decrypted.Vat, FullName: decrypted.FullName());
        }
        public static TraderModel ToTraderModel(this Trader trader)
        {
            return trader.ToModel<TraderModel>();
        }
        public static IList<SrfTraderModel> Decrypt(this IList<SrfTraderModel> srfTraders)
        {
            foreach (var model in srfTraders)
            {
                model.EmployerIkaUserName = CommonHelper.SrfDecrypt(model.EmployerIkaUserName);
                model.EmployerIkaPassword = CommonHelper.SrfDecrypt(model.EmployerIkaPassword);
                model.TaxisUserName = CommonHelper.SrfDecrypt(model.TaxisUserName);
                model.TaxisPassword = CommonHelper.SrfDecrypt(model.TaxisPassword);
                model.OaeeUserName = CommonHelper.SrfDecrypt(model.OaeeUserName);
                model.OaeePassword = CommonHelper.SrfDecrypt(model.OaeePassword);
                model.SepeUserName = CommonHelper.SrfDecrypt(model.SepeUserName);
                model.SepePassword = CommonHelper.SrfDecrypt(model.SepePassword);
                model.SpecialTaxisUserName = CommonHelper.SrfDecrypt(model.SpecialTaxisUserName);
                model.SpecialTaxisPassword = CommonHelper.SrfDecrypt(model.SpecialTaxisPassword);
                model.EfkaUserName = CommonHelper.SrfDecrypt(model.EfkaUserName);
                model.EfkaPassword = CommonHelper.SrfDecrypt(model.EfkaPassword);
                model.LogistikiDataBaseName = CommonHelper.SrfDecrypt(model.LogistikiDataBaseName);
                model.LogistikiUsername = CommonHelper.SrfDecrypt(model.LogistikiUsername);
                model.LogistikiPassword = CommonHelper.SrfDecrypt(model.LogistikiPassword);
                model.LogistikiIpAddress = CommonHelper.SrfDecrypt(model.LogistikiIpAddress);
                model.LogistikiPort = CommonHelper.SrfDecrypt(model.LogistikiPort);
            }

            return srfTraders;
        }
    }
}