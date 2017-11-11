using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PicBook.Web.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using PicBook.ApplicationService;
using PicBook.Repository.EntityFramework;

namespace PicBook.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;


        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult LoginFacebook(string provider)
        {
            string[] providers = new string[] { "Facebook", "Twitter", "Microsoft", "Google" };
            if (!providers.Contains(provider))
                return Redirect(Url.Action("Login", "Account"));


            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("AuthCallback", "Account", new { provider = provider })
            };

            return Challenge(authenticationProperties, provider);
        }

        public async Task<IActionResult> AuthCallback(string provider)
        {
            var facebookIdentity = User.Identities.FirstOrDefault(i => i.AuthenticationType == provider && i.IsAuthenticated);

            if (facebookIdentity == null)
            {
                return Redirect(Url.Action("Login", "Account"));
            }

            IEnumerable<Claim> a = facebookIdentity.Claims;

            await _userService.EnsureUser(facebookIdentity.Claims.ToList());

            return Redirect(Url.Action("Index", "Home"));
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return Redirect(Url.Action("Index", "Home"));
        }
    }
}