using KeluargaTani.Models;
using KeluargaTani.Service;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IUtilService _utilService;
        public ProductController(ILogger<HomeController> logger,
                                IProductService productService,
                                IUtilService utilService)
        {
            _logger = logger;
            _productService = productService;
            _utilService = utilService;
        }

        public IActionResult Product()
        {
            return View();
        }

        public JsonResult ReadProductData()
        {
            try
            {
                var result = _productService.ReadProductData();
                return _utilService.ProcessDataTable(result);
            }catch(Exception ex)
            {
                return Json(new{result = "error"});
            }
        }

        public JsonResult CreateNewProduct(ProductModelDto data)
        {
            try
            {
                var result = _productService.CreateNewProduct(data);
                return Json(new{result = "success"});
            }catch(Exception ex)
            {
                return Json(new{result = "error"});
            }
        }

        [HttpGet]
        public JsonResult GetProductList()
        {
            var result = _productService.GetProductList();
            return Json(result);
        }
    }
}
