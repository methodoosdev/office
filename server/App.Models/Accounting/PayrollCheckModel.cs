using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Accounting
{
    public partial record PayrollCheckSearchModel : BaseNopModel
    {
        public PayrollCheckSearchModel()
        {
            EmployerIds = new List<int>();
        }

        public IList<int> EmployerIds { get; set; }
        public DateTime Periodos { get; set; }
        public int EmployeeId { get; set; }
    }

    public partial record PayrollCheckSearchFormModel : BaseNopModel
    {
    }
    public partial record PayrollCheckModel : BaseNopModel
    {
        public PayrollCheckModel() 
        {
            Items = new List<PayrollCheckModel>();
        }
        public string TraderName { get; set; }
        public string ErrorMessage { get; set; }
        public decimal Jan { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar { get; set; }
        public decimal Apr { get; set; }
        public decimal May { get; set; }
        public decimal Jun { get; set; }
        public decimal Jul { get; set; }
        public decimal Aug { get; set; }
        public decimal Sep { get; set; }
        public decimal Oct { get; set; }
        public decimal Nov { get; set; }
        public decimal Dec { get; set; }
        public List<PayrollCheckModel> Items { get; set; }


        public bool _jan { get; set; }
        public bool _feb { get; set; }
        public bool _mar { get; set; }
        public bool _apr { get; set; }
        public bool _may { get; set; }
        public bool _jun { get; set; }
        public bool _jul { get; set; }
        public bool _aug { get; set; }
        public bool _sep { get; set; }
        public bool _oct { get; set; }
        public bool _nov { get; set; }
        public bool _dec { get; set; }
    }
    public partial record PayrollCheckTableModel : BaseNopModel
    {
    }
    public class PayrollCheckDto
    {
        public decimal Jan { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar{ get; set; }
        public decimal Apr { get; set; }
        public decimal May { get; set; }
        public decimal Jun { get; set; }
        public decimal Jul { get; set; }
        public decimal Aug { get; set; }
        public decimal Sep { get; set; }
        public decimal Oct { get; set; }
        public decimal Nov { get; set; }
        public decimal Dec { get; set; }
    }
}