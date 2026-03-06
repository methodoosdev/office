using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class MonthlyFinancialBulletinPredictionsCreditQuery : TraderFactoryQuery
    {
        public MonthlyFinancialBulletinPredictionsCreditQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
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
