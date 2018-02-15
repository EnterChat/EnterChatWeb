﻿using System;
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

        public IActionResult About()
        {
            return View();
        }

        [Authorize(Policy = "OnlyForAdmin")]
        public IActionResult AdminPanel() {
            return View();
        }

        public IActionResult AccountPanel()
        {
            return View();
        }

        public IActionResult Files()
        {
            return View();
        }

        public IActionResult GroupChat()
        {
            return View();
        }

        public IActionResult Notes()
        {
            return View();
        }

        public IActionResult Topics()
        {
            return View();
        }

        public IActionResult Workers()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //мои контроллеры

        /*public ActionResult GetUserReg()
        {
            return PartialView("_UserReg");
        }

        public ActionResult GetWorkerReg()
        {
            return PartialView("_WorkerReg");
        }*/

        public IActionResult RegisterCompany()
        {
            return RedirectToAction("RegisterCompany", "Account");
        }
    }
}
