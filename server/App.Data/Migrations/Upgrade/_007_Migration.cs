using App.Core.Domain.Offices;
using App.Core.Domain.Traders;
using App.Core.Infrastructure.Mapper;
using FluentMigrator;
using System.Linq;

namespace App.Data.Migrations.Upgrade
{
    [NopMigration("2024-04-06 00:00:00", "007 Encryption")]
    public class _007_Migration : Migration
    {
        private readonly INopDataProvider _dataProvider;

        public _007_Migration(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var accountingOffice = _dataProvider.GetTable<AccountingOffice>().First();

            accountingOffice.TaxisNetUserName = AesEncryption.Encrypt(accountingOffice.TaxisNetUserName);
            accountingOffice.TaxisNetPassword = AesEncryption.Encrypt(accountingOffice.TaxisNetPassword);
            accountingOffice.AadeRegistryUsername = AesEncryption.Encrypt(accountingOffice.AadeRegistryUsername);
            accountingOffice.AadeRegistryPassword = AesEncryption.Encrypt(accountingOffice.AadeRegistryPassword);
            accountingOffice.OfficeUsername = AesEncryption.Encrypt(accountingOffice.OfficeUsername);
            accountingOffice.OfficePassword = AesEncryption.Encrypt(accountingOffice.OfficePassword);
            accountingOffice.SrfUsername = AesEncryption.Encrypt(accountingOffice.SrfUsername);
            accountingOffice.SrfPassword = AesEncryption.Encrypt(accountingOffice.SrfPassword);
            accountingOffice.TaxSystemUsername = AesEncryption.Encrypt(accountingOffice.TaxSystemUsername);
            accountingOffice.TaxSystemPassword = AesEncryption.Encrypt(accountingOffice.TaxSystemPassword);
            accountingOffice.HyperPayrollUsername = AesEncryption.Encrypt(accountingOffice.HyperPayrollUsername);
            accountingOffice.HyperPayrollPassword = AesEncryption.Encrypt(accountingOffice.HyperPayrollPassword);
            accountingOffice.HyperLogUsername = AesEncryption.Encrypt(accountingOffice.HyperLogUsername);
            accountingOffice.HyperLogPassword = AesEncryption.Encrypt(accountingOffice.HyperLogPassword);
            accountingOffice.ProsvasisUsername = AesEncryption.Encrypt(accountingOffice.ProsvasisUsername);
            accountingOffice.ProsvasisPassword = AesEncryption.Encrypt(accountingOffice.ProsvasisPassword);

            _dataProvider.UpdateEntity(accountingOffice);

            var pageIndex = 0;
            var pageSize = 100;

            var query =
                from c in _dataProvider.GetTable<Trader>()
                select c;

            while (true)
            {
                var traders = query.ToPagedListAsync(pageIndex++, pageSize).Result;

                if (!traders.Any())
                    break;

                foreach (var trader in traders)
                {
                    trader.Vat = AesEncryption.Encrypt(trader.Vat);
                    trader.LastName = AesEncryption.Encrypt(trader.LastName);
                    trader.FirstName = AesEncryption.Encrypt(trader.FirstName);

                    // Γενικά στοιχεία
                    trader.CodeName = AesEncryption.Encrypt(trader.CodeName);
                    trader.LastName2 = AesEncryption.Encrypt(trader.LastName2);
                    trader.FatherLastName = AesEncryption.Encrypt(trader.FatherLastName);
                    trader.FatherFirstName = AesEncryption.Encrypt(trader.FatherFirstName);
                    trader.MotherLastName = AesEncryption.Encrypt(trader.MotherLastName);
                    trader.MotherFirstname = AesEncryption.Encrypt(trader.MotherFirstname);
                    trader.IdentityNumber = AesEncryption.Encrypt(trader.IdentityNumber);
                    trader.Amka = AesEncryption.Encrypt(trader.Amka);
                    trader.Gemh = AesEncryption.Encrypt(trader.Gemh);
                    trader.AmIka = AesEncryption.Encrypt(trader.AmIka);
                    trader.AmOaee = AesEncryption.Encrypt(trader.AmOaee);
                    trader.AmOga = AesEncryption.Encrypt(trader.AmOga);
                    trader.AmEtaa = AesEncryption.Encrypt(trader.AmEtaa);

                    trader.AmDiasIka = AesEncryption.Encrypt(trader.AmDiasIka);
                    trader.AmDiasEtea = AesEncryption.Encrypt(trader.AmDiasEtea);
                    trader.AmEmployer = AesEncryption.Encrypt(trader.AmEmployer);
                    trader.AmRetirement = AesEncryption.Encrypt(trader.AmRetirement);
                    trader.AmsOga = AesEncryption.Encrypt(trader.AmsOga);
                    trader.AmsNat = AesEncryption.Encrypt(trader.AmsNat);
                    trader.Eam = AesEncryption.Encrypt(trader.Eam);
                    trader.TradeName = AesEncryption.Encrypt(trader.TradeName);

                    // Στοιχεία έδρας
                    trader.RegisterCode = AesEncryption.Encrypt(trader.RegisterCode);
                    trader.JobAddress = AesEncryption.Encrypt(trader.JobAddress);
                    trader.JobStreetNumber = AesEncryption.Encrypt(trader.JobStreetNumber);
                    trader.JobCity = AesEncryption.Encrypt(trader.JobCity);
                    trader.JobMunicipality = AesEncryption.Encrypt(trader.JobMunicipality);
                    trader.JobPlace = AesEncryption.Encrypt(trader.JobPlace);
                    trader.JobPostcode = AesEncryption.Encrypt(trader.JobPostcode);
                    trader.JobPhoneNumber1 = AesEncryption.Encrypt(trader.JobPhoneNumber1);
                    trader.JobPhoneNumber2 = AesEncryption.Encrypt(trader.JobPhoneNumber2);
                    trader.JobFax = AesEncryption.Encrypt(trader.JobFax);

                    // Στοιχεία οικίας
                    trader.HomeAddress = AesEncryption.Encrypt(trader.HomeAddress);
                    trader.HomeStreetNumber = AesEncryption.Encrypt(trader.HomeStreetNumber);
                    trader.HomeCity = AesEncryption.Encrypt(trader.HomeCity);
                    trader.HomeMunicipality = AesEncryption.Encrypt(trader.HomeMunicipality);
                    trader.HomePlace = AesEncryption.Encrypt(trader.HomePlace);
                    trader.HomePostcode = AesEncryption.Encrypt(trader.HomePostcode);
                    trader.HomePhoneNumber1 = AesEncryption.Encrypt(trader.HomePhoneNumber1);
                    trader.HomePhoneNumber2 = AesEncryption.Encrypt(trader.HomePhoneNumber2);
                    trader.HomeCellphone = AesEncryption.Encrypt(trader.HomeCellphone);
                    trader.HomeFax = AesEncryption.Encrypt(trader.HomeFax);

                    trader.Email = AesEncryption.Encrypt(trader.Email);
                    trader.Email2 = AesEncryption.Encrypt(trader.Email2);
                    trader.Email3 = AesEncryption.Encrypt(trader.Email3);
                    trader.Iban = AesEncryption.Encrypt(trader.Iban);

                    // Στοιχεία νομίμου εκπροσώπου - λογιστή
                    trader.RepresentativeUserName = AesEncryption.Encrypt(trader.RepresentativeUserName);
                    trader.RepresentativePassword = AesEncryption.Encrypt(trader.RepresentativePassword);
                    trader.IbanPeriodicF2 = AesEncryption.Encrypt(trader.IbanPeriodicF2);

                    // Στοιχεία σύνδεσης
                    trader.TaxisUserName = AesEncryption.Encrypt(trader.TaxisUserName);
                    trader.TaxisPassword = AesEncryption.Encrypt(trader.TaxisPassword);
                    trader.TaxisKeyNumber = AesEncryption.Encrypt(trader.TaxisKeyNumber);

                    trader.SpecialTaxisUserName = AesEncryption.Encrypt(trader.SpecialTaxisUserName);
                    trader.SpecialTaxisPassword = AesEncryption.Encrypt(trader.SpecialTaxisPassword);
                    trader.EfkaUserName = AesEncryption.Encrypt(trader.EfkaUserName);
                    trader.EfkaPassword = AesEncryption.Encrypt(trader.EfkaPassword);

                    // Στοιχεία σύνδεσης
                    trader.IntrastatUserName = AesEncryption.Encrypt(trader.IntrastatUserName);
                    trader.IntrastatPassword = AesEncryption.Encrypt(trader.IntrastatPassword);
                    trader.EstateUserName = AesEncryption.Encrypt(trader.EstateUserName);
                    trader.EstatePassword = AesEncryption.Encrypt(trader.EstatePassword);
                    trader.KeaoUserName = AesEncryption.Encrypt(trader.KeaoUserName);
                    trader.KeaoPassword = AesEncryption.Encrypt(trader.KeaoPassword);
                    trader.OaeeUserName = AesEncryption.Encrypt(trader.OaeeUserName);
                    trader.OaeePassword = AesEncryption.Encrypt(trader.OaeePassword);
                    trader.OaeeKeynumber = AesEncryption.Encrypt(trader.OaeeKeynumber);
                    trader.EmployerIkaUserName = AesEncryption.Encrypt(trader.EmployerIkaUserName);
                    trader.EmployerIkaPassword = AesEncryption.Encrypt(trader.EmployerIkaPassword);
                    trader.EmployeeIkaUserName = AesEncryption.Encrypt(trader.EmployeeIkaUserName);
                    trader.EmployeeIkaPassword = AesEncryption.Encrypt(trader.EmployeeIkaPassword);
                    trader.IkaKeyNumber = AesEncryption.Encrypt(trader.IkaKeyNumber);
                    trader.OaedUserName = AesEncryption.Encrypt(trader.OaedUserName);
                    trader.OaedPassword = AesEncryption.Encrypt(trader.OaedPassword);
                    trader.OaedKeyNumber = AesEncryption.Encrypt(trader.OaedKeyNumber);
                    trader.GemhUserName = AesEncryption.Encrypt(trader.GemhUserName);
                    trader.GemhPassword = AesEncryption.Encrypt(trader.GemhPassword);
                    trader.ErmisUserName = AesEncryption.Encrypt(trader.ErmisUserName);
                    trader.ErmisPassword = AesEncryption.Encrypt(trader.ErmisPassword);
                    trader.OpsydUserName = AesEncryption.Encrypt(trader.OpsydUserName);
                    trader.OpsydPassword = AesEncryption.Encrypt(trader.OpsydPassword);
                    trader.SepeUserName = AesEncryption.Encrypt(trader.SepeUserName);
                    trader.SepePassword = AesEncryption.Encrypt(trader.SepePassword);

                    trader.OeeUserName = AesEncryption.Encrypt(trader.OeeUserName);
                    trader.OeePassword = AesEncryption.Encrypt(trader.OeePassword);
                    trader.OpekepeUserName = AesEncryption.Encrypt(trader.OpekepeUserName);
                    trader.OpekepePassword = AesEncryption.Encrypt(trader.OpekepePassword);
                    trader.AgrotiUserName = AesEncryption.Encrypt(trader.AgrotiUserName);
                    trader.AgrotiPassword = AesEncryption.Encrypt(trader.AgrotiPassword);
                    trader.TepahUserName = AesEncryption.Encrypt(trader.TepahUserName);
                    trader.TepahPassword = AesEncryption.Encrypt(trader.TepahPassword);
                    trader.NatUserName = AesEncryption.Encrypt(trader.NatUserName);
                    trader.NatPassword = AesEncryption.Encrypt(trader.NatPassword);

                    trader.DipetheUserName = AesEncryption.Encrypt(trader.DipetheUserName);
                    trader.DipethePassword = AesEncryption.Encrypt(trader.DipethePassword);
                    trader.ModUserName = AesEncryption.Encrypt(trader.ModUserName);
                    trader.ModPassword = AesEncryption.Encrypt(trader.ModPassword);

                    trader.SmokePrdUserName = AesEncryption.Encrypt(trader.SmokePrdUserName);
                    trader.SmokePrdPassword = AesEncryption.Encrypt(trader.SmokePrdPassword);
                    trader.Article39UserName = AesEncryption.Encrypt(trader.Article39UserName);
                    trader.Article39Password = AesEncryption.Encrypt(trader.Article39Password);

                    trader.MhdasoUserName = AesEncryption.Encrypt(trader.MhdasoUserName);
                    trader.MhdasoPassword = AesEncryption.Encrypt(trader.MhdasoPassword);
                    trader.MydataUserName = AesEncryption.Encrypt(trader.MydataUserName);
                    trader.MydataPaswword = AesEncryption.Encrypt(trader.MydataPaswword);
                    trader.MydataApi = AesEncryption.Encrypt(trader.MydataApi);

                    //migration
                    trader.KeaoIkaUserName = AesEncryption.Encrypt(trader.KeaoIkaUserName);
                    trader.KeaoIkaPassword = AesEncryption.Encrypt(trader.KeaoIkaPassword);
                    trader.KeaoOaeeUserName = AesEncryption.Encrypt(trader.KeaoOaeeUserName);
                    trader.KeaoOaeePassword = AesEncryption.Encrypt(trader.KeaoOaeePassword);
                    trader.KeaoEfkaUserName = AesEncryption.Encrypt(trader.KeaoEfkaUserName);
                    trader.KeaoEfkaPassword = AesEncryption.Encrypt(trader.KeaoEfkaPassword);

                    // SoftOne
                    trader.LogistikiDataBaseName = AesEncryption.Encrypt(trader.LogistikiDataBaseName);
                    trader.LogistikiUsername = AesEncryption.Encrypt(trader.LogistikiUsername);
                    trader.LogistikiPassword = AesEncryption.Encrypt(trader.LogistikiPassword);
                    trader.LogistikiIpAddress = AesEncryption.Encrypt(trader.LogistikiIpAddress);
                    trader.LogistikiPort = AesEncryption.Encrypt(trader.LogistikiPort);

                    trader.WebSite = AesEncryption.Encrypt(trader.WebSite);
                }

                _dataProvider.UpdateEntities(traders);
            }

        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
