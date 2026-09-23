using KeluargaTani.Models;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class CalculatorController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public CalculatorController(ILogger<HomeController> logger,
                                ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Calculator()
        {
            return View();
        }
    }
}
