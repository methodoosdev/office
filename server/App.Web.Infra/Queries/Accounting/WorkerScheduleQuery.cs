namespace App.Web.Infra.Queries.Accounting
{
    public static partial class WorkerScheduleQuery
	{
        //Όλοι οι ενεργοι υπάλληλοι
        public static string Workers => @"
			SELECT E.ID_EMP AS WorkerId
				, E.ACTIVE_CARD AS ActiveCard
				, P.ID_CMP AS CompanyId
				, CAST(P.WEEK_DAYS * P.DAY_HOURS AS DECIMAL(10,2)) AS WorkingHours
				, E.CODE AS WorkerCardName
				, CONCAT(E.SURNAME, ' ', E.NAME) AS WorkerName
				, FORMAT(E.VAT, 'D9') AS WorkerVat
			FROM PERIODOI_DATA AS P LEFT OUTER JOIN
				 EMPLOYEE AS E ON P.ID_EMP = E.ID_EMP
			WHERE P.ID_CMP = @pCompanyId 
				AND P.ISACTIVE = 1 
				AND P.ID_PERIODOS = 0
            ";
        public static string Pending => @"
            SELECT T.LastName
				, ISNULL(T.FirstName, '') AS FirstName
				, MAX(W.PeriodToDate) AS PeriodToDate
            FROM Trader T 
				LEFT OUTER JOIN WorkerSchedule W ON T.Id = W.TraderId AND W.PeriodToDate >= @pCurrentToDate
            WHERE T.HasSubmissionSchedules = 1
            GROUP BY T.LastName, T.FirstName
            ORDER BY T.LastName, T.FirstName
        ";
        public static string Names => @"
			SELECT DISTINCT		
				CONCAT(E.SURNAME, ' ', E.NAME) AS [Name]
			FROM PERIODOI_DATA AS P LEFT OUTER JOIN
				 EMPLOYEE AS E ON P.ID_EMP = E.ID_EMP
			WHERE P.ID_CMP = @pCompanyId
				AND P.ISACTIVE = 1 
				AND P.ID_PERIODOS = 0
        ";
    }
}
