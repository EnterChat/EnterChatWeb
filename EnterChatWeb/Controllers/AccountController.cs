using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EnterChatWeb.Data;
using EnterChatWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
                Company companyTest = await _context.Companies.FirstOrDefaultAsync(c => c.Title.Equals(model.CompanyName));
                if (companyTest != null)
                {
                    Worker worker = await _context.Workers.FirstOrDefaultAsync(w => w.InviteCode == model.InviteCode);
                    if (worker != null && worker.CompanyID == companyTest.ID)
                    {
                        Department department = await _context.Departments.FirstOrDefaultAsync(d => d.ID == worker.DepartmentID);
                        User user = await _context.Users.FirstOrDefaultAsync(u => u.WorkerID == worker.ID);
                        worker.Department = department;

                        User user_loginTest = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);

                        if (user_loginTest != null)
                        {
                            ModelState.AddModelError("", "Пользователь с таким логином уже есть!");
                        }

                        if (user == null)
                        {
                            if (user.Login == model.Login)
                            {
                                ModelState.AddModelError("", "Пользователь с таким логином уже существует!");
                            }

                            byte[] salt = GenerateSalt();
                            user = new User
                            {
                                Login = model.Login,
                                RegistrationDate = DateTime.Now,
                                Password = Hashing(model.Password, salt),
                                Email = model.Email,
                                Salt = Convert.ToBase64String(salt),
                                Worker = worker,
                                Company = companyTest
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
                        ModelState.AddModelError("", "Такого кода не существует!");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Такой компании не существует!");
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

                User user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user != null)
                {

                    byte[] user_salt = Convert.FromBase64String(user.Salt);
                    string hash = Hashing(model.Password, user_salt);
                    if (user.Password == hash)
                    {
                        Worker worker = await _context.Workers.FirstOrDefaultAsync(u => u.ID == user.WorkerID);
                        Department department = await _context.Departments.FirstOrDefaultAsync(d => d.ID == worker.DepartmentID);
                        Company company = await _context.Companies.FirstOrDefaultAsync(u => u.ID == user.CompanyID);
                        worker.Department = department;
                        user.Worker = worker;
                        user.Company = company;
                        await Authenticate(user);
                        return RedirectToAction("About", "Data");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
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
                Company companyTest = await _context.Companies.FirstOrDefaultAsync(c => c.Title.Equals(model.Title));
                if (companyTest == null)
                {
                    User user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
                    if (user == null)
                    {
                        //добавляем данные в бд
                        Company company = new Company
                        {
                            Title = model.Title,
                            CreationDate = DateTime.Now,
                            WorkEmail = model.WorkEmail
                        };

                        Department department = new Department
                        {
                            Title = model.DepTitle,
                            Status = true,
                            Company = company
                        };

                        Worker worker = new Worker
                        {
                            FirstName = model.FirstName,
                            SecondName = model.SecondName,
                            //Status = true,
                            InviteCode = null,
                            Company = company,
                            Department = department
                        };
                        byte[] salt = GenerateSalt();
                        user = new User
                        {
                            Email = model.Email,
                            Salt = Convert.ToBase64String(salt),
                            Password = Hashing(model.Password, salt),
                            Login = model.Login,
                            Company = company,
                            Worker = worker,
                            RegistrationDate = DateTime.Now
                        };

                        _context.Users.Add(user);
                        _context.Companies.Add(company);
                        _context.Workers.Add(worker);
                        _context.Departments.Add(department);
                        await _context.SaveChangesAsync();

                        await Authenticate(user);

                        return RedirectToAction("About", "Data");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Компания с таким названием уже зарегистрирована");
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
                new Claim("Status", user.Worker.Department.Status.ToString())
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        private string Hashing(string password, byte[] salt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;

        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
