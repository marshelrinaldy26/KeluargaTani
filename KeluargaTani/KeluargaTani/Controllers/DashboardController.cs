using KeluargaTani.Models;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly KeluargaTani.Service.IDashboardService _dashboardService;
        
        public DashboardController(ILogger<HomeController> logger,
                                ApplicationDbContext db,
                                KeluargaTani.Service.IDashboardService dashboardService)
        {
            _logger = logger;
            _db = db;
            _dashboardService = dashboardService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSummary()
        {
            try
            {
                var result = _dashboardService.GetDashboardSummary();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetChartData(string period)
        {
            try
            {
                var result = _dashboardService.GetChartData(period);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
