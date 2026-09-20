using KeluargaTani.Models;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class SalesController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public SalesController(ILogger<HomeController> logger,
                                ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Sales()
        {
            return View();
        }
    }
}
