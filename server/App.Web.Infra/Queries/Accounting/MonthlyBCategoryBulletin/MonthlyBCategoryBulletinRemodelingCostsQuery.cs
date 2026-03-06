using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class MonthlyBCategoryBulletinRemodelingCostsQuery : TraderFactoryQuery
    {
        public MonthlyBCategoryBulletinRemodelingCostsQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"";

        public override string SoftOne_B => @"
            SELECT A.CODE AS Code, A.NAME AS Description, B.FISCPRD AS Year, S.SODOCV04 AS Rate, 
                CASE 
                    WHEN A.CODE LIKE '7%' THEN SUM(B.LCREDITTMP) - SUM(B.LDEBITTMP)
                ELSE 
                    SUM(B.LDEBITTMP) - SUM(B.LCREDITTMP) 
                END AS Total, 0 AS Result
            FROM ACNT AS A INNER JOIN ACNBALSHEET AS B ON A.ACNT = B.ACNT INNER JOIN
                (SELECT ID, SODOCV04 FROM SODOCTYPE 
                    WHERE SODTYPE = 89 AND FISCPRD IN (@pYear) AND SODOCV04 > 0) AS S ON B.ACNT = S.ID INNER JOIN
                         ACNT ON B.ACNT = ACNT.ACNT
            WHERE 
                LEN(A.CODE) = 18
	            AND ((SUBSTRING(A.CODE, 1, 1) IN ('7','2','6')) OR A.CODE LIKE '80.03.00.0__0.%')
                AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD IN (@pYear)
				AND B.PERIOD >= 0
				AND B.PERIOD <= @pPeriod
                AND (A.SODTYPE = 89) 
                AND S.SODOCV04 > 0
            GROUP BY A.CODE, A.NAME, B.FISCPRD, S.SODOCV04
            ORDER BY A.CODE
        ";

        public override string Prosvasis => @"
            SELECT A.CODE AS Code, A.NAME AS Description, B.FISCPRD AS Year, S.SODOCV04 AS Rate, 
                CASE 
                    WHEN A.CODE LIKE '7%' THEN SUM(B.LCREDITTMP) - SUM(B.LDEBITTMP)
                ELSE 
                    SUM(B.LDEBITTMP) - SUM(B.LCREDITTMP) 
                END AS Total, 0 AS Result
            FROM ACNT AS A INNER JOIN ACNBALSHEET AS B ON A.ACNT = B.ACNT INNER JOIN
                (SELECT ID, SODOCV04 FROM SODOCTYPE 
                    WHERE SODTYPE = 89 AND FISCPRD IN (@pYear) AND SODOCV04 > 0) AS S ON B.ACNT = S.ID INNER JOIN
                         ACNT ON B.ACNT = ACNT.ACNT
            WHERE 
                LEN(A.CODE) = 18
	            AND ((SUBSTRING(A.CODE, 1, 1) IN ('7','2','6')) OR A.CODE LIKE '80.03.00.0__0.%')
                AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD IN (@pYear)
				AND B.PERIOD >= 0
				AND B.PERIOD <= @pPeriod
                AND (A.SODTYPE = 89) 
                AND S.SODOCV04 > 0
            GROUP BY A.CODE, A.NAME, B.FISCPRD, S.SODOCV04
            ORDER BY A.CODE
        ";

        public override string Prosvasis_C => @"
            SELECT A.CODE AS Code, A.NAME AS Description, B.FISCPRD AS Year, S.SODOCV04 AS Rate, 
                CASE 
                    WHEN A.CODE LIKE '7%' THEN SUM(B.LCREDITTMP) - SUM(B.LDEBITTMP)
                ELSE 
                    SUM(B.LDEBITTMP) - SUM(B.LCREDITTMP) 
                END AS Total, 0 AS Result
            FROM ACNT AS A INNER JOIN ACNBALSHEET AS B ON A.ACNT = B.ACNT INNER JOIN
                (SELECT ID, SODOCV04 FROM SODOCTYPE 
                    WHERE SODTYPE = 89 AND FISCPRD IN (@pYear) AND SODOCV04 > 0) AS S ON B.ACNT = S.ID INNER JOIN
                         ACNT ON B.ACNT = ACNT.ACNT
            WHERE 
                LEN(A.CODE) = 18
	            AND ((SUBSTRING(A.CODE, 1, 1) IN ('7','2','6')) OR A.CODE LIKE '80.03.00.0__0.%')
                AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD IN (@pYear)
				AND B.PERIOD >= 0
				AND B.PERIOD <= @pPeriod
                AND (A.SODTYPE = 89) 
                AND S.SODOCV04 > 0
            GROUP BY A.CODE, A.NAME, B.FISCPRD, S.SODOCV04
            ORDER BY A.CODE
        ";
    }
}
