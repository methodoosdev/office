using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class MonthlyFinancialBulletinPredictionsResultQuery : TraderFactoryQuery
    {
        public MonthlyFinancialBulletinPredictionsResultQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
            SELECT @pId AS Id, @pDescription AS [Description], A.FISCPRD AS Year, A.PERIOD AS Periodos,
                @pLevel AS Level, '00' AS Branch, CAST( SUM(L.QTY * L.PRICE - L.LINEVAL) AS DECIMAL(18, 2)) AS Total, 0 AS Result
            FROM FINDOC A INNER JOIN
                MTRLINES L ON A.FINDOC = L.FINDOC INNER JOIN
                MTRL M ON L.MTRL = M.MTRL
            WHERE A.COMPANY IN (@pCompanyId)
                AND A.FISCPRD IN (@pPreviousYear, @pCurrentYear)
                AND A.PERIOD >= 0 AND A.PERIOD <= @pPeriod
	            AND A.FPRMS IN ({0}) 
	            AND A.SOSOURCE = 1251 
	            AND A.SODTYPE = 12
            GROUP BY A.PERIOD, A.FISCPRD
        ";

        public override string SoftOne_B => @"
            SELECT @pId AS Id, @pDescription AS [Description], A.FISCPRD AS Year, A.PERIOD AS Periodos,
                @pLevel AS Level, '00' AS Branch, SUM(ISNULL(A.SUMAMNT,0)) AS Total, 0 AS Result
            FROM FINDOC A
            WHERE A.COMPANY IN (@pCompanyId)
                AND A.FISCPRD IN (@pPreviousYear, @pCurrentYear)
                AND A.PERIOD >= 0 AND A.PERIOD <= @pPeriod
	            AND A.FPRMS IN ({0}) 
	            AND A.SOSOURCE = 1251 
	            AND A.SODTYPE = 12
            GROUP BY A.PERIOD, A.FISCPRD
        ";

        public override string Prosvasis => @"
            SELECT @pId AS Id, @pDescription AS [Description], A.FISCPRD AS Year, A.PERIOD AS Periodos,
                @pLevel AS Level, '00' AS Branch, SUM(ISNULL(A.SUMAMNT,0)) AS Total, 0 AS Result
            FROM FINDOC A
            WHERE A.COMPANY IN (@pCompanyId)
                AND A.FISCPRD IN (@pPreviousYear, @pCurrentYear)
                AND A.PERIOD >= 0 AND A.PERIOD <= @pPeriod
	            AND A.FPRMS IN ({0}) 
	            AND A.SOSOURCE = 1251 
	            AND A.SODTYPE = 12
            GROUP BY A.PERIOD, A.FISCPRD
        ";

        public override string Prosvasis_C => @"
            SELECT @pId AS Id, @pDescription AS [Description], A.FISCPRD AS Year, A.PERIOD AS Periodos,
                @pLevel AS Level, '00' AS Branch, SUM(ISNULL(A.SUMAMNT,0)) AS Total, 0 AS Result
            FROM FINDOC A
            WHERE A.COMPANY IN (@pCompanyId)
                AND A.FISCPRD IN (@pPreviousYear, @pCurrentYear)
                AND A.PERIOD >= 0 AND A.PERIOD <= @pPeriod
	            AND A.FPRMS IN ({0}) 
	            AND A.SOSOURCE = 1251 
	            AND A.SODTYPE = 12
            GROUP BY A.PERIOD, A.FISCPRD
        ";
    }
}
