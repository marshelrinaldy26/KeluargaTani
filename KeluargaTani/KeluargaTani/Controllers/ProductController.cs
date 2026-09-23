using KeluargaTani.Models;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public ProductController(ILogger<HomeController> logger,
                                ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Product()
        {
            return View();
        }

        // public JsonResult CreateNewProduct()
        // {
        //     try
        //     {
        //         return 
        //     }catch(Exception ex)
        //     {
                
        //     }
        // }
    }
}
