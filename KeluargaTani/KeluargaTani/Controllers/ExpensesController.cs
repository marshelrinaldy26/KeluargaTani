using KeluargaTani.Models;
using KeluargaTani.Service;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExpensesService _ExpensesService;
        private readonly IUtilService _utilService;
        public ExpensesController(ILogger<HomeController> logger,
                                ApplicationDbContext db,
                                IExpensesService ExpensesService,
                                IUtilService utilService) 
        {
            _logger = logger;
            _ExpensesService = ExpensesService;
            _utilService = utilService;
        }

        public IActionResult Expenses()
        {
            return View();
        }

        public JsonResult ReadExpensesData()
        {
            try
            {
                var result = _ExpensesService.ReadExpensesData();
                return _utilService.ProcessDataTable(result);
            }
            catch(Exception ex)
            {
                return Json(new{result = "error"});
            }
        }

        public JsonResult CreateNewExpense(ExpensesModelDto data)
        {
            try
            {
                var result = _ExpensesService.CreateNewExpense(data);
                return Json(new{result = "success"});
            }
            catch(Exception ex)
            {
                return Json(new{result = "error"});
            }
        }
    }
}
