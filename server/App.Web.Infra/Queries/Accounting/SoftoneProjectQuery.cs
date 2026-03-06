namespace App.Web.Infra.Queries.Accounting
{
    public static partial class SoftoneProjectQuery
    {
        public static string All => @"
			SELECT 
				A.PRJC AS Id,
				A.CODE AS Code, 
				A.NAME AS Description, 
				A.ISACTIVE AS Active, 
				B.NAME AS Customer,
				A.CRDDATE AS CreatedDate,
				A.FROMDATE AS StartingDate,
				A.CLOSEDATE AS EndingDate
			FROM PRJC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
			WHERE A.COMPANY IN (@pCompanyId) AND A.SODTYPE=40 ORDER BY A.CRDDATE DESC
        ";
    }
}
