using KeluargaTani.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Diagnostics;

namespace KeluargaTani.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ILogger<HomeController> logger,
                                ApplicationDbContext db,
                                UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> LoginAccount(LoginModel loginModel)
        {
            if (loginModel == null || string.IsNullOrWhiteSpace(loginModel.username) || string.IsNullOrWhiteSpace(loginModel.password))
            {
                return Json(new { success = false, message = "Username dan password tidak boleh kosong." });
            }
            var userLogin = await _userManager.FindByNameAsync(loginModel.username);
            if (userLogin != null)
            {
                var passwordCheck = await _signInManager.CheckPasswordSignInAsync(userLogin, loginModel.password, false);
                if (passwordCheck.Succeeded)
                {
                    var result = await _signInManager.PasswordSignInAsync(userLogin, loginModel.password, false, false);
                    if (result.Succeeded)
                    {
                        var user = _db.Users.FirstOrDefault(x => x.UserName == loginModel.username);
                        if (user == null)
                        {
                            return Json(new { success = false, message = "Akun Tidak Ditemukan" });
                        }

                        return Json(new { success = true});
                    }
                }
                return Json(new { success = false, message = "Wrong credentials. Please try again." });
            }
            return Json(new { success = false, message = "Wrong credentials. Please try again." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser 
            { 
                UserName = model.Username, 
                Email = model.Email 
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "Account created successfully!" });
            }

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { errors });
        }
    }
}
