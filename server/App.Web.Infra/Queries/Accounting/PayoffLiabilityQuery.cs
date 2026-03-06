namespace App.Web.Infra.Queries.Accounting
{
    public static partial class PayoffLiabilityQuery
    {
        public static string All => @"
            SELECT 
				A.CODE AS Id, 
				A.NAME AS Description,
				B.PERIOD AS Period,
                CASE 
                    WHEN 
                        SUBSTRING(A.CODE, 1, 1) = '4' OR 
                        SUBSTRING(A.CODE, 1, 1) = '5' OR  
                        SUBSTRING(A.CODE, 1, 1) = '7' 
                    THEN CAST(SUM(B.LCREDITTMP - B.LDEBITTMP) AS DECIMAL(18,2))
                ELSE 
                    CAST(SUM(B.LDEBITTMP - B.LCREDITTMP) AS DECIMAL(18,2))
                END AS Value,
				CAST(SUM(B.LDEBITTMP) AS DECIMAL(18,2)) as Debit, 
				CAST(SUM(B.LCREDITTMP) AS DECIMAL(18,2)) AS Credit
            FROM ACNT A, ACNBALSHEET B
            WHERE A.ACNT = B.ACNT
                AND A.ACNSCHEMA = @pSchema
                AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD IN (@pYear)
                AND A.SODTYPE = 89
                AND B.PERIOD >= 0
                AND B.PERIOD <= 12
				AND ({0})
			GROUP BY A.CODE, A.NAME, B.PERIOD, A.ACNGRADE
			HAVING A.ACNGRADE = (MAX(A.ACNGRADE))
        ";
    }
}
