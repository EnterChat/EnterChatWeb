using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnterChatWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace EnterChatWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult About()
        {
            return View();
        }

        [Authorize(Policy = "OnlyForAdmin")]
        public IActionResult AdminPanel() {
            return View();
        }
        [Authorize]
        public IActionResult AccountPanel()
        {
            return View();
        }
        [Authorize]
        public IActionResult Files()
        {
            return View();
        }
        [Authorize]
        public IActionResult GroupChat()
        {
            return View();
        }
        [Authorize]
        public IActionResult Notes()
        {
            return View();
        }
        [Authorize]
        public IActionResult Topics()
        {
            return View();
        }
        [Authorize]
        public IActionResult Workers()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            return RedirectToAction("Login", "Account");
        }

        public IActionResult RegisterCompany()
        {
            return RedirectToAction("RegisterCompany", "Account");
        }

        public IActionResult RegisterWorker()
        {
            return RedirectToAction("RegisterWorker", "Account");
        }
    }
}
