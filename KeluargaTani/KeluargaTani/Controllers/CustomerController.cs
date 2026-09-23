using KeluargaTani.Models;
using KeluargaTani.Service;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomerService _customerService;
        private readonly IUtilService _utilService;
        public CustomerController(ILogger<HomeController> logger,
                                ApplicationDbContext db,
                                ICustomerService customerService,
                                IUtilService utilService) 
        {
            _logger = logger;
            _customerService = customerService;
            _utilService = utilService;
        }

        public IActionResult Customer()
        {
            return View();
        }

        public JsonResult TambahPembeliBaru(PembeliModel data)
        {
            try
            {
                var result = _customerService.TambahPembeliBaru(data);
                return Json(new{result = "success"});
            }catch(Exception ex)
            {
                return Json(new{result = "error"});
            }
        }

        [HttpPost]
        public JsonResult ReadCustomerData()
        {
            try
            {
                var result = _customerService.ReadCustomerData();
                return _utilService.ProcessDataTable(result);
            }catch(Exception ex)
            {
                return Json(new{result = "error"});
            }
        }
    }
}
