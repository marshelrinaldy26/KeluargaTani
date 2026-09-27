using KeluargaTani.Models;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly KeluargaTani.Service.IReportService _reportService;
        private readonly KeluargaTani.Service.IUtilService _utilService;
        
        public ReportController(ILogger<HomeController> logger,
                                ApplicationDbContext db,
                                KeluargaTani.Service.IReportService reportService,
                                KeluargaTani.Service.IUtilService utilService)
        {
            _logger = logger;
            _db = db;
            _reportService = reportService;
            _utilService = utilService;
        }

        public IActionResult Report()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetReportData(string filter)
        {
            try
            {
                var result = _reportService.GetReportSummary(filter);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
