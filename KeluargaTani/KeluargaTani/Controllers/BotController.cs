using KeluargaTani.Models;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class BotController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public BotController(ILogger<HomeController> logger,
                                ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Bot()
        {
            return View();
        }
    }
}
