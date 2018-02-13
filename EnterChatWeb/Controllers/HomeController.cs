﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnterChatWeb.Models;

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
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //мои контроллеры\
        [HttpGet]
        public ViewResult RegisterCompany()
        {
            return View();
        }
        [HttpPost]
        public ViewResult RegisterCompany(Company company)
        {
            if (ModelState.IsValid)
            {
                return View("About");
            }
            else
            {
                return View();
            }
        }
    }
}
