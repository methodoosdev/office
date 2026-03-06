namespace App.Web.Infra.Queries.Common
{
    public static partial class OfficeDBQuery
    {
        public static string TraderRatingByTrader => @"
            SELECT 
                CONCAT(e.LastName, ' ', e.FirstName) AS [Employee],
                CONCAT(t.LastName, ' ', t.FirstName) AS [Trader],
                SUM(g.Gravity) AS [Value]
            FROM 
                TraderRating g
            JOIN 
                TraderRatingTraderMapping tm ON g.Id = tm.TraderRatingId
            JOIN 
                Trader t ON tm.TraderId = t.Id
            JOIN 
                TraderRatingCategory c ON g.TraderRatingCategoryId = c.Id
            JOIN 
                Employee e ON g.DepartmentId = e.DepartmentId
            JOIN 
                TraderEmployeeMapping em ON e.Id = em.EmployeeId AND tm.TraderId = em.TraderId
            JOIN 
                Department d ON e.DepartmentId = d.Id
            GROUP BY 
                CONCAT(e.LastName, ' ', e.FirstName), 
                CONCAT(t.LastName, ' ', t.FirstName)
        ";
        public static string TraderActivityLog => @"
            SELECT
                C.Email AS UserName, 
                C.NickName AS NickName, 
                ISNULL(T.LastName,'') + ' ' + ISNULL(T.FirstName,'') AS TraderName, 
                A.Name AS ActivityLogType, 
                COUNT(A.Name) AS ActivityCount
            FROM
                ActivityLog AS L 
            INNER JOIN
                Customer AS C ON L.CustomerId = C.Id 
            INNER JOIN
                ActivityLogType AS A ON L.ActivityLogTypeId = A.Id 
            INNER JOIN
                Trader AS T ON C.TraderId = T.Id
            WHERE  
                (C.TraderId > 0 AND L.CreatedOnUtc >= @pFrom) AND (L.CreatedOnUtc <= @pTo)
            GROUP BY 
                C.Email, C.NickName, ISNULL(T.LastName,'') + ' ' + ISNULL(T.FirstName,''), A.Name
            ORDER BY ActivityLogType
        ";
        public static string EmployeeActivityLog => @"
            SELECT
                C.Email AS UserName, 
                C.NickName AS NickName, 
                ISNULL(E.LastName,'') + ' ' + ISNULL(E.FirstName,'') AS EmployeeName, 
                A.Name AS ActivityLogType, 
                COUNT(A.Name) AS ActivityCount
            FROM
                ActivityLog AS L 
            INNER JOIN
                Customer AS C ON L.CustomerId = C.Id 
            INNER JOIN
                ActivityLogType AS A ON L.ActivityLogTypeId = A.Id 
            INNER JOIN
                Employee AS E ON C.EmployeeId = E.Id
            WHERE  
                (C.EmployeeId > 0 AND L.CreatedOnUtc >= @pFrom) AND (L.CreatedOnUtc <= @pTo)
            GROUP BY 
                C.Email, C.NickName, ISNULL(E.LastName,'') + ' ' + ISNULL(E.FirstName,''), A.Name
            ORDER BY ActivityLogType
        ";
        public static string SummaryTableQuery => @"
		    WITH DepartmentAggregates AS (
			    SELECT 
			    e.DepartmentId, t.Vat, t.CategoryBookTypeId,
			    CONCAT(t.LastName, ' ', t.FirstName) AS [Trader],
			    CONCAT(e.LastName, ' ', e.FirstName) AS [Employee],
			    SUM(g.Gravity) AS [Gravity],
			    SUM(SUM(g.Gravity)) OVER (PARTITION BY CONCAT(e.LastName, ' ', e.FirstName)) AS [TotalGravity]
		    FROM 
			    TraderRating g
		    JOIN 
			    TraderRatingTraderMapping tm ON g.Id = tm.TraderRatingId
		    JOIN 
			    Trader t ON tm.TraderId = t.Id
		    JOIN 
			    Employee e ON g.DepartmentId = e.DepartmentId
		    JOIN 
			    TraderEmployeeMapping em ON e.Id = em.EmployeeId AND tm.TraderId = em.TraderId
		    JOIN 
			    Department d ON e.DepartmentId = d.Id
		    GROUP BY 
			    e.DepartmentId, t.Vat, t.CategoryBookTypeId,
			    CONCAT(e.LastName, ' ', e.FirstName), 
			    CONCAT(t.LastName, ' ', t.FirstName)
		    )
		    SELECT 
			    Trader, Vat, CategoryBookTypeId,
    
			    -- Department 2 Columns
			    ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN Employee END), '') AS Employee_Dep2,
			    ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN Gravity END), 0) AS Gravity_Dep2,
			    ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN TotalGravity END), 0) AS TotalGravity_Dep2,

			    -- Department 3 Columns
			    ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN Employee END), '') AS Employee_Dep3,
			    ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN Gravity END), 0) AS Gravity_Dep3,
			    ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN TotalGravity END), 0) AS TotalGravity_Dep3
		    FROM 
			    DepartmentAggregates
		    GROUP BY 
			    Trader, Vat, CategoryBookTypeId
        ";

        public static string ValuationTableQuery => @"
            WITH DepartmentAggregates AS (
                SELECT 
                    e.DepartmentId, t.Id,
                    e.EmployeeSalary AS [EmployeeSalary],
                    t.TraderPayment AS [TraderPayment],
                    CONCAT(t.LastName, ' ', t.FirstName) AS [Trader],
                    CONCAT(e.LastName, ' ', e.FirstName) AS [Employee],
                    s.Description AS [SpecialtyName],
                    SUM(g.Gravity) AS [Gravity],
                    SUM(SUM(g.Gravity)) OVER (PARTITION BY CONCAT(e.LastName, ' ', e.FirstName)) AS [TotalGravity]
                FROM 
                    TraderRating g
                JOIN 
                    TraderRatingTraderMapping tm ON g.Id = tm.TraderRatingId
                JOIN 
                    Trader t ON tm.TraderId = t.Id
                JOIN 
                    Employee e ON g.DepartmentId = e.DepartmentId
                JOIN 
                    TraderEmployeeMapping em ON e.Id = em.EmployeeId AND tm.TraderId = em.TraderId
                JOIN 
                    Department d ON e.DepartmentId = d.Id
                LEFT OUTER JOIN 
                    Specialty s ON e.SpecialtyId = s.Id
                WHERE 
                    t.Active = 1 AND t.Deleted = 0 AND t.CategoryBookTypeId > 0
                GROUP BY 
                    e.DepartmentId, t.Id,
                    CONCAT(e.LastName, ' ', e.FirstName), 
                    CONCAT(t.LastName, ' ', t.FirstName),
                    e.EmployeeSalary,
                    t.TraderPayment,
                    s.Description -- Group by Specialty Description
            )
            SELECT 
                Trader, Id,
                TraderPayment,
                ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN SpecialtyName END), 'Βοηθός λογιστή') AS SpecialtyName,

                -- Department 2 Columns
                ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN Employee END), 'Υπεύθ.Μισθοδοσίας') AS Employee_Dep2,
                ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN EmployeeSalary END), 0) AS EmployeeSalary_Dep2,
                ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN Gravity END), 0) AS Gravity_Dep2,
                ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN TotalGravity END), 0) AS TotalGravity_Dep2,

                -- Department 3 Columns
                ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN Employee END), 'Κενό') AS Employee_Dep3,
                ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN EmployeeSalary END), 0) AS EmployeeSalary_Dep3,
                ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN Gravity END), 0) AS Gravity_Dep3,
                ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN TotalGravity END), 0) AS TotalGravity_Dep3,

                -- Department 7 Columns
                ISNULL(MAX(CASE WHEN DepartmentId = 7 THEN Employee END), 'Κενό') AS Employee_Dep7,
                ISNULL(MAX(CASE WHEN DepartmentId = 7 THEN EmployeeSalary END), 0) AS EmployeeSalary_Dep7,
                ISNULL(MAX(CASE WHEN DepartmentId = 7 THEN Gravity END), 0) AS Gravity_Dep7,
                ISNULL(MAX(CASE WHEN DepartmentId = 7 THEN TotalGravity END), 0) AS TotalGravity_Dep7,

                -- Department 8 Columns
                ISNULL(MAX(CASE WHEN DepartmentId = 8 THEN Employee END), 'Κενό') AS Employee_Dep8,
                ISNULL(MAX(CASE WHEN DepartmentId = 8 THEN EmployeeSalary END), 0) AS EmployeeSalary_Dep8,
                ISNULL(MAX(CASE WHEN DepartmentId = 8 THEN Gravity END), 0) AS Gravity_Dep8,
                ISNULL(MAX(CASE WHEN DepartmentId = 8 THEN TotalGravity END), 0) AS TotalGravity_Dep8
            FROM 
                DepartmentAggregates
            GROUP BY 
                Trader, Id, TraderPayment
            ORDER BY 
                Employee_Dep2;

        ";

        public static string ValuationTableByCategoryBookQuery => @"
            WITH DepartmentAggregates AS (
                SELECT 
                    e.DepartmentId, t.Vat, t.CategoryBookTypeId,
                    e.EmployeeSalary AS [EmployeeSalary],
	                t.TraderPayment AS [TraderPayment],
                    CONCAT(t.LastName, ' ', t.FirstName) AS [Trader],
                    CONCAT(e.LastName, ' ', e.FirstName) AS [Employee],
                    SUM(g.Gravity) AS [Gravity],
                    SUM(SUM(g.Gravity)) OVER (PARTITION BY CONCAT(e.LastName, ' ', e.FirstName)) AS [TotalGravity]
                FROM 
                    TraderRating g
                JOIN 
                    TraderRatingTraderMapping tm ON g.Id = tm.TraderRatingId
                JOIN 
                    Trader t ON tm.TraderId = t.Id
                JOIN 
                    Employee e ON g.DepartmentId = e.DepartmentId
                JOIN 
                    TraderEmployeeMapping em ON e.Id = em.EmployeeId AND tm.TraderId = em.TraderId
                LEFT OUTER JOIN 
                    Department d ON e.DepartmentId = d.Id
                WHERE 
                    t.Active = 1 AND t.Deleted = 0 AND t.CategoryBookTypeId > 0
                GROUP BY 
                    e.DepartmentId, t.Vat, t.CategoryBookTypeId,
                    CONCAT(e.LastName, ' ', e.FirstName), 
                    CONCAT(t.LastName, ' ', t.FirstName),
                        e.EmployeeSalary,
		                t.TraderPayment
                )
                SELECT 
                    Trader, Vat, CategoryBookTypeId,TraderPayment, '' AS CategoryBook,
    
                    -- Department 2 Columns
                    ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN Employee END), 'Κενό') AS Employee_Dep2,
                    ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN EmployeeSalary END), 0) AS EmployeeSalary_Dep2,
                    ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN Gravity END), 0) AS Gravity_Dep2,
                    ISNULL(MAX(CASE WHEN DepartmentId = 2 THEN TotalGravity END), 0) AS TotalGravity_Dep2,

                    -- Department 3 Columns
                    ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN Employee END), 'Κενό') AS Employee_Dep3,
                    ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN EmployeeSalary END), 0) AS EmployeeSalary_Dep3,
                    ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN Gravity END), 0) AS Gravity_Dep3,
                    ISNULL(MAX(CASE WHEN DepartmentId = 3 THEN TotalGravity END), 0) AS TotalGravity_Dep3,
	
                    -- Department 3 Columns
                    ISNULL(MAX(CASE WHEN DepartmentId = 7 THEN Employee END), 'Κενό') AS Employee_Dep7,
                    ISNULL(MAX(CASE WHEN DepartmentId = 7 THEN EmployeeSalary END), 0) AS EmployeeSalary_Dep7,
                    ISNULL(MAX(CASE WHEN DepartmentId = 7 THEN Gravity END), 0) AS Gravity_Dep7,
                    ISNULL(MAX(CASE WHEN DepartmentId = 7 THEN TotalGravity END), 0) AS TotalGravity_Dep7,
	
                    -- Department 3 Columns
                    ISNULL(MAX(CASE WHEN DepartmentId = 8 THEN Employee END), 'Κενό') AS Employee_Dep8,
                    ISNULL(MAX(CASE WHEN DepartmentId = 8 THEN EmployeeSalary END), 0) AS EmployeeSalary_Dep8,
                    ISNULL(MAX(CASE WHEN DepartmentId = 8 THEN Gravity END), 0) AS Gravity_Dep8,
                    ISNULL(MAX(CASE WHEN DepartmentId = 8 THEN TotalGravity END), 0) AS TotalGravity_Dep8
                FROM 
                    DepartmentAggregates
                GROUP BY 
                    Trader, Vat,TraderPayment, CategoryBookTypeId
	                order by Employee_Dep2
            ";
    }
}
