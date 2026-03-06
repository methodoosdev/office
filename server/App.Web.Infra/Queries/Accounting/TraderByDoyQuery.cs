using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class TraderByDoyQuery : TraderFactoryQuery
    {
        public TraderByDoyQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
            SELECT A.NAME AS LastName,
				A.NAME2 AS FirstName,
				A.NAME3 AS FatherName,
				A.SOUNIT AS Activity,
				A.ADDRESS AS [Address],
				A.City AS City,
				A.ZIP AS PostCode,
				A.AFM AS Afm,
				A.PHONE1 AS Phone,
				B.NAME AS Doy,
 				B.CODE AS DoyId
			FROM COMPANY A 
				LEFT OUTER JOIN IRSDATA B ON B.IRSDATA = A.IRSDATA 
			WHERE A.COMPANY = @pCompanyId
        ";

        public override string SoftOne_B => @"
            SELECT A.NAME AS LastName,
				A.NAME2 AS FirstName,
				A.NAME3 AS FatherName,
				A.SOUNIT AS Activity,
				A.ADDRESS AS [Address],
				A.City AS City,
				A.ZIP AS PostCode,
				A.AFM AS Afm,
				A.PHONE1 AS Phone,
				B.NAME AS Doy,
 				B.CODE AS DoyId
			FROM COMPANY A 
				LEFT OUTER JOIN IRSDATA B ON B.IRSDATA = A.IRSDATA 
			WHERE A.COMPANY = @pCompanyId
        ";

        public override string Prosvasis => @"
            SELECT A.NAME AS LastName,
				A.NAME2 AS FirstName,
				A.NAME3 AS FatherName,
				A.SOUNIT AS Activity,
				A.ADDRESS AS [Address],
				A.City AS City,
				A.ZIP AS PostCode,
				A.AFM AS Afm,
				A.PHONE1 AS Phone,
				B.NAME AS Doy,
 				B.CODE AS DoyId
			FROM COMPANY A 
				LEFT OUTER JOIN IRSDATA B ON B.IRSDATA = A.IRSDATA 
			WHERE A.COMPANY = @pCompanyId
        ";

        public override string Prosvasis_C => @"
            SELECT A.NAME AS LastName,
				A.NAME2 AS FirstName,
				A.NAME3 AS FatherName,
				A.SOUNIT AS Activity,
				A.ADDRESS AS [Address],
				A.City AS City,
				A.ZIP AS PostCode,
				A.AFM AS Afm,
				A.PHONE1 AS Phone,
				B.NAME AS Doy,
 				B.CODE AS DoyId
			FROM COMPANY A 
				LEFT OUTER JOIN IRSDATA B ON B.IRSDATA = A.IRSDATA 
			WHERE A.COMPANY = @pCompanyId
        ";
    }
}
