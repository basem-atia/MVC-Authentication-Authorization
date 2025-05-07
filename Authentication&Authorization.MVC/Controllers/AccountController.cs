using Authentication_Authorization.MVC.Data.Models;
using Authentication_Authoriztion.MVC.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Authentication_Authorization.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public AccountController(
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto RegisterUser)
        {
            if (!ModelState.IsValid)
                return View(RegisterUser);
            var user = new UserModel
            {
                UserName = RegisterUser.userName,
                Email = RegisterUser.userEmail,
                Role = "User"
            };
            var creationResult = await _userManager.CreateAsync(user, RegisterUser.password);
            Console.WriteLine(user.Id);
            if (!creationResult.Succeeded)
            {
                foreach (var error in creationResult.Errors)
                {
                    // mbn7otsh key fe addmodelerror ashan el resala mtzhrsh
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(RegisterUser);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Role,user.Role)
            };
            var claimResult = _userManager.AddClaimsAsync(user, claims);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }
            var user = await _userManager.FindByEmailAsync(loginDto.userEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View(loginDto);
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.password);
            if (!passwordValid)
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View(loginDto);
            }
            var claims = await _userManager.GetClaimsAsync(user);
            await _signInManager.SignInWithClaimsAsync(user, false, claims);
            return RedirectToAction("Logout");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}
