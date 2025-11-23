using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models;
using MVC_Project.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MVC_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthClientService _authService;
        public AccountController(IAuthClientService authService)
        {
            _authService = authService;
        }

        // GET: LoginView
        [HttpGet]
        public IActionResult LoginView()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginView(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
            var response = await _authService.LoginAsync(loginViewModel);
            string returnUrl = null;

            if (response)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Ivalid login attempt.");
            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}
