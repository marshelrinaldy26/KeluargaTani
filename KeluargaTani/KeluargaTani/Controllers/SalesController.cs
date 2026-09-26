using KeluargaTani.Models;
using KeluargaTani.Service;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KeluargaTani.Controllers
{
    public class SalesController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly ISalesService _salesService;
        private readonly IUtilService _utilService;
        public SalesController(ILogger<HomeController> logger,
                                ApplicationDbContext db,
                                ISalesService salesService,
                                IUtilService utilService)
        {
            _logger = logger;
            _db = db;
            _salesService = salesService;
            _utilService = utilService;
        }

        public IActionResult Sales()
        {
            return View();
        }

        public JsonResult ReadSalesData()
        {
            try
            {
                var result = _salesService.ReadSalesData();
                return _utilService.ProcessDataTable(result);
            }
            catch (Exception ex)
            {
                return Json(new { result = "error" });
            }
        }

        [HttpPost]
        public JsonResult CreateNewSales(SalesModelDto data)
        {
            try
            {
                var result = _salesService.CreateNewSales(data);
                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error" });
            }
        }
    }
}
