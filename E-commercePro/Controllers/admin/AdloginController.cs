using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;
using E_commercePro.Models;

namespace E_commercePro.Controllers.admin
{
    public class AdloginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

          

            return View();
        }


        [HttpPost]
         public async Task<IActionResult> Login(ALModel modelLogin)
        {

            if (modelLogin.UserName == "shaahir" && modelLogin.Password == "12345")
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier,modelLogin.UserName),
                    new Claim("OtherProperties","Example Rol")
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = modelLogin.KeepLogedIn
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);

                return RedirectToAction("Dashboard", "AdDashboard");
               
            }
            ViewData["ValidateMessage"] = "user not found";

            return View();
        }
    }
}
