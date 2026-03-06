namespace App.Web.Infra.Queries.Accounting
{
    public static partial class VatTransferenceQuery
        {
            public static string VatTransference => @"
                SELECT  B.name AS Vat, b.vat AS Code, B.MTRTYPE,A.PERIOD,SUM(A.IMPVAL - A.EXPVAL) AS REMAINVAL,  '1' AS TYPE
                FROM MTRBALSHEET A INNER JOIN 
                    MTRL B ON A.MTRL=B.MTRL inner join Branch Br on br.COMPANY = b.COMPANY
                WHERE A.COMPANY = @pCompanyId
                    AND A.FISCPRD = @pYear
                    AND A.PERIOD >= @pPeriodFrom
                    AND A.PERIOD <= @pPeriodTo
                    AND B.MTRTYPE IN (2,4) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
                    AND b.SODTYPE=61 AND b.CODE LIKE '2%'
                    GROUP BY B.name, b.vat, A.MTRL,B.MTRTYPE,A.PERIOD
                
                union
                
                SELECT B.NAME AS Vat, b.vat AS Code, B.MTRTYPE,A.PERIOD,SUM(A.IMPVAL - A.EXPVAL) AS REMAINVAL,  '0' AS TYPE
                FROM MTRBALSHEET A INNER JOIN 
                    MTRL B ON A.MTRL=B.MTRL inner join   Branch Br on br.COMPANY = b.COMPANY
                WHERE A.COMPANY = @pCompanyId
                    AND A.FISCPRD = @pYear
                    AND A.PERIOD >= @pPeriodFrom
                    AND A.PERIOD <= @pPeriodTo
                    AND B.MTRTYPE IN (1,3) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
                    AND b.SODTYPE=61 AND b.CODE LIKE '7%'
                GROUP BY  B.NAME, b.vat, A.MTRL,B.MTRTYPE,A.PERIOD";
        }
    }
