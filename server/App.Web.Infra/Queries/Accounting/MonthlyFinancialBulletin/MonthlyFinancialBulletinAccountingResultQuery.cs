using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class MonthlyFinancialBulletinAccountingResultQuery : TraderFactoryQuery
    {
        public MonthlyFinancialBulletinAccountingResultQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
            SELECT A.CODE AS Id, A.NAME AS Description, B.FISCPRD AS Year, B.PERIOD AS Periodos,
                A.ACNGRADE AS Level,  
                CASE 
                    WHEN SUBSTRING(A.CODE, 1, 1) = '7' THEN SUBSTRING(A.CODE, 11, 2)
                    WHEN SUBSTRING(A.CODE, 1, 1) = '2' THEN SUBSTRING(A.CODE, 10, 2)
                    WHEN SUBSTRING(A.CODE, 1, 1) = '6' THEN 
					CASE 
                        WHEN SUBSTRING(A.CODE, 1, 5) = '60.01' THEN SUBSTRING(A.CODE, 10, 2)
                        WHEN SUBSTRING(A.CODE, 1, 5) = '60.02' THEN SUBSTRING(A.CODE, 10, 2)
						WHEN SUBSTRING(A.CODE, 1, 8) = '64.11.02' THEN SUBSTRING(A.CODE, 12, 2)
						ELSE SUBSTRING(A.CODE, 11, 2)
					END
                    WHEN SUBSTRING(A.CODE, 1, 1) = '8' THEN SUBSTRING(A.CODE, 11, 2)
                ELSE 
                    'ERROR' 
                END AS Branch, 
                CASE 
                    WHEN A.CODE LIKE '7%' THEN SUM(B.LCREDITTMP) - SUM(B.LDEBITTMP)
                ELSE 
                    SUM(B.LDEBITTMP) - SUM(B.LCREDITTMP) 
                END AS Total, 0 AS Result
            FROM ACNT A, ACNBALSHEET B
            WHERE A.ACNT = B.ACNT
	            AND LEN(A.CODE) >= 18
	            AND ((SUBSTRING(A.CODE, 1, 1) IN('7','2','6')) OR A.CODE LIKE '80.03.00.0__0.%')
                AND A.ACNSCHEMA = @pSchema
                AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD IN (@pPreviousYear, @pCurrentYear)
                AND A.SODTYPE = 89
                AND B.PERIOD >= 0
                AND B.PERIOD <= @pPeriod
            GROUP BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
            ORDER BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
        ";

        public override string SoftOne_B => @"
            SELECT MTRN.CODE AS Id, MTRN.NAME AS Description, A.FISCPRD AS Year, A.PERIOD AS Periodos, 5 AS Level,   
	            CASE 
		            WHEN SUBSTRING(MTRN.CODE, 1, 1) = '7' THEN SUBSTRING(MTRN.CODE, 11, 2)
		            WHEN SUBSTRING(MTRN.CODE, 1, 1) = '2' THEN SUBSTRING(MTRN.CODE, 10, 2)
		            WHEN SUBSTRING(MTRN.CODE, 1, 1) = '6' THEN 
		            CASE 
			            WHEN SUBSTRING(MTRN.CODE, 1, 2) = '60' THEN SUBSTRING(MTRN.CODE, 7, 2)
			            WHEN SUBSTRING(MTRN.CODE, 1, 8) = '64.11.02' THEN SUBSTRING(MTRN.CODE, 12, 2)
			            ELSE SUBSTRING(MTRN.CODE, 11, 2)
		            END
                    WHEN SUBSTRING(MTRN.CODE, 1, 1) = '8' THEN SUBSTRING(MTRN.CODE, 11, 2)
	            ELSE 
		            'ERROR' 
	            END AS Branche, SUM(MTRN.SPCTYPESUM) AS Total, 0 AS Result
            FROM FINDOC A LEFT JOIN TRDR ON A.TRDR=TRDR.TRDR,
	            (SELECT MTRN.FINDOC, SUM(MTRN.LTRNVAL * (TPRMS.FLG02 - TPRMS.FLG05)) AS SPCTYPESUM, MTRL.CODE, MTRL.NAME
	            FROM MTRTRN MTRN, MTRL MTRL, TPRMS TPRMS, SOCLMNS SC
	            WHERE MTRN.MTRL = MTRL.MTRL
	                AND LEN(MTRL.CODE) = 18
		            AND MTRN.COMPANY = TPRMS.COMPANY
		            AND MTRN.SODTYPE = TPRMS.SODTYPE
		            AND MTRN.TPRMS = TPRMS.TPRMS
		            AND MTRN.COMPANY IN (@pCompanyId)
		            AND MTRN.SODTYPE = 61
		            AND MTRN.SOSOURCE in (1261, 1361)		
		            AND MTRL.SOCLMNS=SC.SOCLMNS
		            AND SC.COMPANY = MTRN.COMPANY
		            AND SC.ISACTIVE=1
		            AND SC.ISPRINT=1
		            AND SC.SOTYPE=1
	            GROUP BY MTRN.FINDOC, MTRL.CODE, MTRL.NAME) MTRN
            WHERE A.FINDOC = MTRN.FINDOC
	            AND A.COMPANY IN (@pCompanyId)
                AND A.SOSOURCE IN (1261, 1361)
	            --AND A.BRANCH IN (@pCompanyId) 
	            --AND SUBSTRING(MTRN.CODE, 1, 1) IN('7','2','6')
	            AND ((SUBSTRING(MTRN.CODE, 1, 1) IN('7','2','6')) OR MTRN.CODE LIKE '80.03.00.0__0.%')
	            AND A.FISCPRD IN (@pPreviousYear, @pCurrentYear)
                AND A.PERIOD >= 0
                AND A.PERIOD <= @pPeriod
            GROUP BY MTRN.CODE, MTRN.NAME, A.FISCPRD, A.PERIOD
            ORDER BY MTRN.CODE
        ";

        public override string Prosvasis => @"
            SELECT MTRN.CODE AS Id, MTRN.NAME AS Description, A.FISCPRD AS Year, A.PERIOD AS Periodos, 5 AS Level,   
	            CASE 
		            WHEN SUBSTRING(MTRN.CODE, 1, 1) = '7' THEN SUBSTRING(MTRN.CODE, 11, 2)
		            WHEN SUBSTRING(MTRN.CODE, 1, 1) = '2' THEN SUBSTRING(MTRN.CODE, 10, 2)
		            WHEN SUBSTRING(MTRN.CODE, 1, 1) = '6' THEN 
		            CASE 
			            WHEN SUBSTRING(MTRN.CODE, 1, 2) = '60' THEN SUBSTRING(MTRN.CODE, 7, 2)
			            WHEN SUBSTRING(MTRN.CODE, 1, 8) = '64.11.02' THEN SUBSTRING(MTRN.CODE, 12, 2)
			            ELSE SUBSTRING(MTRN.CODE, 11, 2)
		            END
                    WHEN SUBSTRING(MTRN.CODE, 1, 1) = '8' THEN SUBSTRING(MTRN.CODE, 11, 2)
	            ELSE 
		            'ERROR' 
	            END AS Branche, SUM(MTRN.SPCTYPESUM) AS Total, 0 AS Result
            FROM FINDOC A LEFT JOIN TRDR ON A.TRDR=TRDR.TRDR,
	            (SELECT MTRN.FINDOC, SUM(MTRN.LTRNVAL * (TPRMS.FLG02 - TPRMS.FLG05)) AS SPCTYPESUM, MTRL.CODE, MTRL.NAME
	            FROM MTRTRN MTRN, MTRL MTRL, TPRMS TPRMS, SOCLMNS SC
	            WHERE MTRN.MTRL = MTRL.MTRL
	                AND LEN(MTRL.CODE) = 18
		            AND MTRN.COMPANY = TPRMS.COMPANY
		            AND MTRN.SODTYPE = TPRMS.SODTYPE
		            AND MTRN.TPRMS = TPRMS.TPRMS
		            AND MTRN.COMPANY IN (@pCompanyId)
		            AND MTRN.SODTYPE = 61
		            AND MTRN.SOSOURCE in (1261, 1361)		
		            AND MTRL.SOCLMNS=SC.SOCLMNS
		            AND SC.COMPANY = MTRN.COMPANY
		            AND SC.ISACTIVE=1
		            AND SC.ISPRINT=1
		            AND SC.SOTYPE=1
	            GROUP BY MTRN.FINDOC, MTRL.CODE, MTRL.NAME) MTRN
            WHERE A.FINDOC = MTRN.FINDOC
	            AND A.COMPANY IN (@pCompanyId)
                AND A.SOSOURCE IN (1261, 1361)
	            --AND A.BRANCH IN (@pCompanyId)
	            --AND SUBSTRING(MTRN.CODE, 1, 1) IN('7','2','6')
	            AND ((SUBSTRING(MTRN.CODE, 1, 1) IN('7','2','6')) OR MTRN.CODE LIKE '80.03.00.0__0.%')
	            AND A.FISCPRD IN (@pPreviousYear, @pCurrentYear)
                AND A.PERIOD >= 0
                AND A.PERIOD <= @pPeriod
            GROUP BY MTRN.CODE, MTRN.NAME, A.FISCPRD, A.PERIOD
            ORDER BY MTRN.CODE
        ";

        public override string Prosvasis_C => @"
            SELECT A.CODE AS Id, A.NAME AS Description, B.FISCPRD AS Year, B.PERIOD AS Periodos,
                A.ACNGRADE AS Level,  
                CASE 
                    WHEN SUBSTRING(A.CODE, 1, 1) = '7' THEN SUBSTRING(A.CODE, 11, 2)
                    WHEN SUBSTRING(A.CODE, 1, 1) = '2' THEN SUBSTRING(A.CODE, 10, 2)
                    WHEN SUBSTRING(A.CODE, 1, 1) = '6' THEN 
					CASE 
                        WHEN SUBSTRING(A.CODE, 1, 5) = '60.01' THEN SUBSTRING(A.CODE, 10, 2)
                        WHEN SUBSTRING(A.CODE, 1, 5) = '60.02' THEN SUBSTRING(A.CODE, 10, 2)
						WHEN SUBSTRING(A.CODE, 1, 8) = '64.11.02' THEN SUBSTRING(A.CODE, 12, 2)
						ELSE SUBSTRING(A.CODE, 11, 2)
					END
                    WHEN SUBSTRING(A.CODE, 1, 1) = '8' THEN SUBSTRING(A.CODE, 11, 2)
                ELSE 
                    'ERROR' 
                END AS Branch, 
                CASE 
                    WHEN A.CODE LIKE '7%' THEN SUM(B.LCREDITTMP) - SUM(B.LDEBITTMP)
                ELSE 
                    SUM(B.LDEBITTMP) - SUM(B.LCREDITTMP) 
                END AS Total, 0 AS Result
            FROM ACNT A, ACNBALSHEET B
            WHERE A.ACNT = B.ACNT
	            AND LEN(A.CODE) >= 18
	            AND ((SUBSTRING(A.CODE, 1, 1) IN('7','2','6')) OR A.CODE LIKE '80.03.00.0__0.%')
                AND A.ACNSCHEMA = @pSchema
                AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD IN (@pPreviousYear, @pCurrentYear)
                AND A.SODTYPE = 89
                AND B.PERIOD >= 0
                AND B.PERIOD <= @pPeriod
            GROUP BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
            ORDER BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
        ";
    }
}
