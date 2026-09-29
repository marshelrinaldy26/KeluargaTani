using KeluargaTani.Models;
using KeluargaTani.Service;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class SettingController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly ISettingService _settingService;

        public SettingController(ILogger<HomeController> logger,
                                ApplicationDbContext db,
                                ISettingService settingService)
        {
            _logger = logger;
            _db = db;
            _settingService = settingService;
        }

        public IActionResult Setting()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSettingData()
        {
            try
            {
                var data = _settingService.GetSetting();
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveSettingData(SettingModelVM vm)
        {
            try
            {
                var result = _settingService.UpdateSetting(vm);
                if (result)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "Gagal menyimpan pengaturan" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
