using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Accounting
{
    public partial record ArticlesCheckSearchModel : BaseNopModel
    {
        public ArticlesCheckSearchModel()
        {
            EmployerIds = new List<int>();
        }

        public IList<int> EmployerIds { get; set; }
        public int NglId { get; set; }
        public DateTime Periodos { get; set; }
        public int EmployeeId { get; set; }
    }

    public partial record ArticlesCheckSearchFormModel : BaseNopModel
    {
    }
    public partial record ArticlesCheckModel : BaseNopModel
    {
        public ArticlesCheckModel()
        {
            Items = new List<ArticlesCheckModel>();
        }
        public int EmployerId { get; set; }
        public string Employer { get; set; }
        public string ErrorMessage { get; set; }
        public decimal Total { get; set; }
        public decimal Salary { get; set; }
        public decimal Fmy { get; set; }
        public decimal Teka { get; set; }
        public decimal Efka { get; set; }
        public List<ArticlesCheckModel> Items { get; set; }

        public bool _total { get; set; }
        public bool _salary { get; set; }
        public bool _fmy { get; set; }
        public bool _teka { get; set; }
        public bool _efka { get; set; }
    }
    public partial record ArticlesCheckTableModel : BaseNopModel
    {
    }
    public class ArticlesCheckEmployeeId
    {
        public int ID_EMP { get; set; }
    }
    public class ArticlesCheckDto
    {
        public decimal Total { get; set; }
        public decimal Salary { get; set; }
        public decimal Fmy { get; set; }
        public decimal Teka { get; set; }
        public decimal Efka { get; set; }
    }
    public class ArticlesCheckDetail
    {
        public string AccountingCode { get; set; }
        public string AccountingCodeName { get; set; }
        public decimal Debit { get; set; }
        public decimal Gredit { get; set; }
    }
    public partial record ArticlesCheckAccountTableModel : BaseNopModel
    {
    }
    public class ArticlesCheckCodeModel
    {
        public string AccountingCode { get; set; }
    }
    public record ArticlesCheckAccountModel : BaseNopModel
    {
        public string Group { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public bool FormatValid { get; set; }
        public bool SchemaValid { get; set; }
        public int NglId { get; set; }
        public string NglName { get; set; }
        public string Date { get; set; }
    }
    public class ArticlesCheckAccount
    {
        public int NGL_GROUP1 { get; set; }
        public string NGL_GROUP2 { get; set; }
        public string LOGARIASMOS { get; set; }
        public string LOGARIASMOS_DSC { get; set; }
        public decimal XREOSI { get; set; }
        public decimal PISTOSI { get; set; }
    }
    public class ArticlesCheckStatus
    {
        public decimal FMY { get; set; }
        public decimal KATHARESAPODOXES { get; set; }
        public decimal PROKATAVOLI { get; set; }
        public decimal XARTOSHMO { get; set; }
        public decimal PLIROTEO { get; set; }
        public decimal S_KOSTOS { get; set; }
        public decimal Efka { get; set; }
        public decimal Teka { get; set; }
    }
}