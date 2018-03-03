using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EnterChatWeb.Data;
using EnterChatWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnterChatWeb.Controllers
{
    public class AccountController : Controller
    {

        private EnterChatContext _context;

        public AccountController(EnterChatContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult RegisterCompany()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterWorker()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterWorker(WorkerRegModel model)
        {
            if (ModelState.IsValid)
            {
                Worker worker = await _context.Workers.FirstOrDefaultAsync(w => w.InviteCode == model.InviteCode);
                if (worker != null)
                {
                    User user = await _context.Users.FirstOrDefaultAsync(u => u.ID == worker.ID);
                    if (user == null)
                    {
                        Company company = await _context.Companies.FirstOrDefaultAsync(c => c.ID == worker.CompanyID);
                        user = new User
                        {
                            Login = model.Login,
                            RegistrationDate = DateTime.Now,
                            Password = model.Password,
                            Email = model.Email,
                            Worker = worker,
                            Company = company
                        };
                        _context.Users.Add(user);

                        await _context.SaveChangesAsync();

                        await Authenticate(user);

                        return RedirectToAction("About", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Такой код уже использован");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Такого кода не существует");
                }
                
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login &&
                u.Password == model.Password);
                if (user != null)
                {
                    Worker worker = await _context.Workers.FirstOrDefaultAsync(u => u.ID == user.WorkerID);
                    Company company = await _context.Companies.FirstOrDefaultAsync(u => u.ID == user.CompanyID);
                    user.Worker = worker;
                    user.Company = company;
                    await Authenticate(user);
                    return RedirectToAction("About", "Data");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCompany(RegModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    //добавляем данные в бд
                    Company company = new Company
                    {
                        Title = model.Title,
                        CreationDate = DateTime.Now,
                        WorkEmail = model.WorkEmail
                    };
                    Worker worker = new Worker
                    {
                        FirstName = model.FirstName,
                        SecondName = model.SecondName,
                        Status = true,
                        InviteCode = null,
                        Company = company
                    };
                    user = new User
                    {
                        Email = model.Email,
                        Password = model.Password,
                        Login = model.Login,
                        Company = company,
                        Worker = worker
                    };

                    _context.Users.Add(user);
                    _context.Companies.Add(company);
                    _context.Workers.Add(worker);
                    await _context.SaveChangesAsync();

                    await Authenticate(user);

                    return RedirectToAction("About", "Data");
                }
                else
                {
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim("FirstName", user.Worker.FirstName),
                new Claim("SecondName", user.Worker.SecondName),
                new Claim("Company", user.Company.Title),
                new Claim("CompanyID", user.CompanyID.ToString()),
                new Claim("Status", user.Worker.Status.ToString())
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
