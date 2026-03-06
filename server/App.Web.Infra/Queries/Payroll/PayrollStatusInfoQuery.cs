namespace App.Web.Infra.Queries.Payroll
{
    public static partial class PayrollStatusInfoQuery
    {
        public static string Get => @"
            SELECT 
                    CONCAT(SURNAME,+' '+NAME) AS 'Employee',
                    CAST(START_DATE AS date) AS 'HireDate',
                    SPC_DESCR AS 'Specialty',
                                    (CASE WHEN TEMP_DATA.EMP_KIND=0 THEN 'ΕΜΜΙΣΘΟΣ'
	                                      WHEN TEMP_DATA.EMP_KIND=1 THEN 'ΗΜΕΡΟΜΙΣΘΙΟΣ'
	                                      WHEN TEMP_DATA.EMP_KIND=2 THEN 'ΩΡΟΜΙΣΘΙΟΣ'
                                    END) AS 'EmployeeType',
                    CONTRACT_TYPE AS 'ContractType', 
                    CAST(CON_START_DATE AS date) AS 'FixedContractStartDate',
                    CAST(CON_END_DATE AS date) AS 'FixedContractEndDate',
                    SALARY_AGREED as 'SalaryAgreed',
                    --EMP_PERCENT as 'emp_perc',
                    --ERG_PERCENT as 'ERG_PERCENT',
                    --EPIDOTHSH_ERGAZ as 'epidoth',
                    --'' as 'ΣΥΜΦΩΝΗΘΕΝ ΜΕ ΔΩΡΑ & ΕΠΙΔΟΜΑΤΑ',
                    --DAYS_BY_WORK_TYPE,
                    --DAYS_TYPE, 

                    (CASE WHEN DAYS_TYPE=1 THEN ((SALARY_AGREED*14)/12/EPIDOTHSH_ERGAZ)
	                    ELSE ((((SALARY_AGREED/DAYS_BY_WORK_TYPE)/EPIDOTHSH_ERGAZ)*14)/12) END )  AS 'W_HourlyWages',
  
                    (CASE WHEN DAYS_TYPE=1 THEN (((SALARY_AGREED/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12 
	                    ELSE ((SALARY_AGREED/DAYS_BY_WORK_TYPE)*14)/12 END ) AS 'W_Salary',
 
                    (CASE WHEN DAYS_TYPE=1 THEN ((((SALARY_AGREED/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12)*(EMP_PERCENT) 
	                    ELSE (((SALARY_AGREED/DAYS_BY_WORK_TYPE)*14)/12)*EMP_PERCENT END)  AS 'W_EmployeeContribution',
 
                    (CASE WHEN DAYS_TYPE=1 THEN ((((SALARY_AGREED/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12)*(ERG_PERCENT/100) 
	                    ELSE (((SALARY_AGREED/DAYS_BY_WORK_TYPE)*14)/12)*(ERG_PERCENT/100) END)  AS 'W_EmployerContribution',
 
                    (CASE WHEN DAYS_TYPE=1 THEN ((((SALARY_AGREED/EPIDOTHSH_ERGAZ)*14)/12)*DAY_HOURS)-(((((SALARY_AGREED/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12)*(EMP_PERCENT)) 
	                    ELSE (((SALARY_AGREED/DAYS_BY_WORK_TYPE)*14)/12)-((((SALARY_AGREED/DAYS_BY_WORK_TYPE)*14)/12)*EMP_PERCENT) END ) AS 'W_NetSalaryPreTax',
 
                    (CASE WHEN DAYS_TYPE=1 THEN ((((SALARY_AGREED/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12)+ ((((SALARY_AGREED/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12)*(ERG_PERCENT/100) 
	                    ELSE (((SALARY_AGREED/DAYS_BY_WORK_TYPE)*14)/12)+ ((((SALARY_AGREED/DAYS_BY_WORK_TYPE)*14)/12)*(ERG_PERCENT/100)) END) AS 'W_Cost',
 
                    --'' as 'ΝΟΜΙΜΑ  ΜΕ  ΔΩΡΑ & ΕΠΙΔΟΜΑΤΑ',
 
                    (CASE WHEN DAYS_TYPE=1 THEN ((SALARY*14)/12/EPIDOTHSH_ERGAZ)
	                    ELSE ((((SALARY/DAYS_BY_WORK_TYPE)/EPIDOTHSH_ERGAZ)*14)/12) END )  AS 'G_HourlyWages',
 
                    (CASE WHEN DAYS_TYPE=1 THEN (((SALARY/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12 
	                    ELSE ((SALARY/DAYS_BY_WORK_TYPE)*14)/12 END ) AS 'G_Salary',
 
                    (CASE WHEN DAYS_TYPE=1 THEN ((((SALARY/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12)*(EMP_PERCENT) 
	                    ELSE (((SALARY/DAYS_BY_WORK_TYPE)*14)/12)*EMP_PERCENT END)  AS 'G_EmployeeContribution',
 
                    (CASE WHEN DAYS_TYPE=1 THEN ((((SALARY_AGREED/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12)*(ERG_PERCENT/100) 
	                    ELSE (((SALARY/DAYS_BY_WORK_TYPE)*14)/12)*(ERG_PERCENT/100) END)  AS 'G_EmployerContribution',
 
                    (CASE WHEN DAYS_TYPE=1 THEN ((((SALARY/EPIDOTHSH_ERGAZ)*14)/12)*DAY_HOURS)-(((((SALARY/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12)*(EMP_PERCENT)) 
	                    ELSE (((SALARY/DAYS_BY_WORK_TYPE)*14)/12)-((((SALARY/DAYS_BY_WORK_TYPE)*14)/12)*EMP_PERCENT) END ) AS 'G_NetSalaryPreTax',
 
                    (CASE WHEN DAYS_TYPE=1 THEN ((((SALARY/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12)+ ((((SALARY/EPIDOTHSH_ERGAZ)*DAY_HOURS)*14)/12)*(ERG_PERCENT/100) 
	                    ELSE (((SALARY/DAYS_BY_WORK_TYPE)*14)/12)+ ((((SALARY/DAYS_BY_WORK_TYPE)*14)/12)*(ERG_PERCENT/100)) END) AS 'G_Cost',
  
                    CHILDREN 
            FROM
                    (SELECT
                    EM.ID_EMP,
                    EM.CODE,
                    EM.SURNAME,
                    EM.NAME,
                    PD.CHILDREN,
                    PD.EMP_KIND,
                    HRDATE AS START_DATE,
                    SPC_DESCR,
                    SALARY_AGREED,
                    SALARY,
                    EPIDOTHSH_ERGAZ,
                    (CASE WHEN PD.EMP_KIND=0 THEN 1 ELSE 8 END) AS DAY_HOURS,
                    (CASE WHEN PD.EMP_KIND=1 THEN 22
	                      WHEN PD.EMP_KIND=0 THEN 25
                    END) AS DAYS_BY_WORK_TYPE,
                    (CASE WHEN PD.EMP_KIND=1 THEN 1
	                      WHEN PD.EMP_KIND=0 THEN 25
                    END) AS DAYS_TYPE,
                    (SELECT TOP 1 ST_DATE FROM EMP_SYMVASEIS_DATES WHERE ID_EMP=EM.ID_EMP ORDER BY PKAUTOINC DESC) AS CON_START_DATE ,
                    (SELECT TOP 1 END_DATE FROM EMP_SYMVASEIS_DATES WHERE ID_EMP=EM.ID_EMP ORDER BY PKAUTOINC DESC) AS CON_END_DATE ,
                    ((select SUM(KRAT.CALC_PERC_ERGAZ) from PERIODOS_APOD APOD
                    INNER JOIN PER_KRATISEIS KRAT ON KRAT.ID_PERIODOS_APOD=APOD.ID_PERIODOS_APOD
                    where  ID_EMP=EM.ID_EMP and XRISI=YEAR(GETDATE()) AND ID_APODOXHS=1 AND ID_PERIODOS=MONTH(GETDATE())-1)) AS EMP_PERCENT,
                    (select SUM(KRAT.CALC_PERC_ERGOD*100) from PERIODOS_APOD APOD
                    INNER JOIN PER_KRATISEIS KRAT ON KRAT.ID_PERIODOS_APOD=APOD.ID_PERIODOS_APOD
                    where  ID_EMP=EM.ID_EMP and XRISI=YEAR(GETDATE()) AND ID_APODOXHS=1 AND ID_PERIODOS=MONTH(GETDATE())-1) AS ERG_PERCENT,
                    (CASE WHEN PD.ORISMENOU=0 THEN 'ΟΡΙΣΜΕΝΟΥ'
	                      WHEN PD.ORISMENOU=1 THEN 'ΑΟΡΙΣΤΟΥ'
                    END) AS CONTRACT_TYPE
                    FROM PERIODOI_DATA PD
                    INNER JOIN EMPLOYEE EM ON PD.id_emp=EM.ID_EMP
 
            WHERE   PD.id_cmp= @pCompanyId AND PD.ISACTIVE= 1 
                    AND ID_PERIODOS = 0 
                    ) AS TEMP_DATA

            ORDER BY SURNAME";
    }
}
