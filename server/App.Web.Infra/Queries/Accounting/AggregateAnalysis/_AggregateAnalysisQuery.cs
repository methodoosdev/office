using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class _AggregateAnalysisQuery : TraderFactoryQuery
    {
        public _AggregateAnalysisQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
            SELECT A.TRNDATE AS Date, D.CODE AS Id, 
                CASE 
                    WHEN D.CODE LIKE '7%' OR D.CODE LIKE '08%' OR D.CODE LIKE '4%' OR D.CODE LIKE '5%' THEN SUM(A.CREDIT) - SUM(A.DEBIT)
                ELSE 
                    SUM(A.DEBIT) - SUM(A.CREDIT) 
                END AS Total
            FROM ((ACNTRN A LEFT OUTER JOIN ACNEDIT B ON A.ACNEDIT=B.ACNEDIT) 
                LEFT OUTER JOIN ACNHEADER C ON A.ACNEDIT=C.ACNEDIT AND A.ACNHEADER=C.ACNHEADER) 
                LEFT OUTER JOIN ACNT D ON A.ACNT=D.ACNT 
            WHERE A.COMPANY = @pCompanyId 
                AND A.SODTYPE = 89 
                AND A.TRNDATE >= @pStartDate
                AND A.TRNDATE <= @pEndDate
                AND A.ISCANCEL IN (0,2) 
            GROUP BY A.TRNDATE, D.CODE
            ORDER BY A.TRNDATE
        ";

        public override string SoftOne_B => @"
            SELECT A.TRNDATE AS Date, D.CODE AS Id, 
                CASE 
                    WHEN D.CODE LIKE '7%' OR D.CODE LIKE '08%' OR D.CODE LIKE '4%' OR D.CODE LIKE '5%' THEN SUM(A.CREDIT) - SUM(A.DEBIT)
                ELSE 
                    SUM(A.DEBIT) - SUM(A.CREDIT) 
                END AS Total
            FROM ((ACNTRN A LEFT OUTER JOIN ACNEDIT B ON A.ACNEDIT=B.ACNEDIT) 
                LEFT OUTER JOIN ACNHEADER C ON A.ACNEDIT=C.ACNEDIT AND A.ACNHEADER=C.ACNHEADER) 
                LEFT OUTER JOIN ACNT D ON A.ACNT=D.ACNT 
            WHERE A.COMPANY = @pCompanyId 
                AND A.SODTYPE = 89 
                AND A.TRNDATE >= @pStartDate
                AND A.TRNDATE <= @pEndDate
                AND A.ISCANCEL IN (0,2) 
            GROUP BY A.TRNDATE, D.CODE
            ORDER BY A.TRNDATE
        ";

        public override string Prosvasis => @"
            SELECT A.TRNDATE AS Date, D.CODE AS Id, 
                CASE 
                    WHEN D.CODE LIKE '7%' OR D.CODE LIKE '08%' OR D.CODE LIKE '4%' OR D.CODE LIKE '5%' THEN SUM(A.CREDIT) - SUM(A.DEBIT)
                ELSE 
                    SUM(A.DEBIT) - SUM(A.CREDIT) 
                END AS Total
            FROM ((ACNTRN A LEFT OUTER JOIN ACNEDIT B ON A.ACNEDIT=B.ACNEDIT) 
                LEFT OUTER JOIN ACNHEADER C ON A.ACNEDIT=C.ACNEDIT AND A.ACNHEADER=C.ACNHEADER) 
                LEFT OUTER JOIN ACNT D ON A.ACNT=D.ACNT 
            WHERE A.COMPANY = @pCompanyId 
                AND A.SODTYPE = 89 
                AND A.TRNDATE >= @pStartDate
                AND A.TRNDATE <= @pEndDate
                AND A.ISCANCEL IN (0,2) 
            GROUP BY A.TRNDATE, D.CODE
            ORDER BY A.TRNDATE
        ";

        public override string Prosvasis_C => @"
            SELECT A.TRNDATE AS Date, D.CODE AS Id, 
                CASE 
                    WHEN D.CODE LIKE '7%' OR D.CODE LIKE '08%' OR D.CODE LIKE '4%' OR D.CODE LIKE '5%' THEN SUM(A.CREDIT) - SUM(A.DEBIT)
                ELSE 
                    SUM(A.DEBIT) - SUM(A.CREDIT) 
                END AS Total
            FROM ((ACNTRN A LEFT OUTER JOIN ACNEDIT B ON A.ACNEDIT=B.ACNEDIT) 
                LEFT OUTER JOIN ACNHEADER C ON A.ACNEDIT=C.ACNEDIT AND A.ACNHEADER=C.ACNHEADER) 
                LEFT OUTER JOIN ACNT D ON A.ACNT=D.ACNT 
            WHERE A.COMPANY = @pCompanyId 
                AND A.SODTYPE = 89 
                AND A.TRNDATE >= @pStartDate
                AND A.TRNDATE <= @pEndDate
                AND A.ISCANCEL IN (0,2) 
            GROUP BY A.TRNDATE, D.CODE
            ORDER BY A.TRNDATE
        ";
    }
}
