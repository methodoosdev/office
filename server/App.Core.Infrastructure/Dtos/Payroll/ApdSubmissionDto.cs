namespace App.Core.Infrastructure.Dtos.Payroll
{
    public class ApdSubmissionDto
    {
        public bool Found { get; set; }
        public int Year { get; set; }
        public string PdfText { get; set; }
        //public decimal ApdTotal { get; set; }
    }
}
