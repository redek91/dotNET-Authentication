﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            // Define claims that identifies the entity
            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "bob@fmail.com"),
                new Claim("Grandma.Says", "Very nice boi.")
            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob K Foo"),
                new Claim("DrivinigLicense", "A+")
            };

            // Define who grants you the claim
            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            // Define the principal of the entity
            var userPrincipal = new ClaimsPrincipal(new [] { grandmaIdentity, licenseIdentity });

            //Set the principal to the context
            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }
    }
}
