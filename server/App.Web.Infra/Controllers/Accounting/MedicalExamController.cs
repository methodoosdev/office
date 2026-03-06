using App.Models.Accounting;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Accounting.Controllers
{
    public partial class MedicalExamController : BaseProtectController
    {
        private readonly IMedicalExamFactory _medicalExamFactory;

        public MedicalExamController(
            IMedicalExamFactory medicalExamFactory)
        {
            _medicalExamFactory = medicalExamFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var tableModel = await _medicalExamFactory.PrepareMedicalExamTableModelAsync(new MedicalExamTableModel());

            return Json(new { tableModel });
        }
    }
}