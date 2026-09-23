using KeluargaTani.Models;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class SettingController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public SettingController(ILogger<HomeController> logger,
                                ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Setting()
        {
            return View();
        }
    }
}
