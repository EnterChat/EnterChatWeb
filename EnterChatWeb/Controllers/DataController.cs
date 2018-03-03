using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EnterChatWeb.Data;
using EnterChatWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnterChatWeb.Controllers
{
    public class DataController : Controller
    {
        private EnterChatContext _context;
        private IHostingEnvironment _appEnvironment;

        public DataController(EnterChatContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _appEnvironment = hostingEnvironment;
        }

        [Authorize]
        public async Task<IActionResult> About()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ID == comp_id);
            return View(company);
        }

        [Authorize(Policy = "OnlyForAdmin")]
        public async Task<IActionResult> AdminPanel()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            var workers = await _context.Workers.Where(x => x.CompanyID == comp_id).ToListAsync();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ID == comp_id);
            /*AdminPanelModel model = new AdminPanelModel();
            model.Company = company;
            model.Workers = workers;*/
            company.Workers = workers;
            return View(company);
        }

        [Authorize]
        public async Task<IActionResult> AccountPanel()
        {
            int user_id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user = await _context.Users.FirstOrDefaultAsync(u => u.ID == user_id);
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> EditUser(int? id)
        {
            if (id != null)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.ID == id);
                if (user != null)
                {
                    return View(user);
                }
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {
            User user_db = await _context.Users.FirstOrDefaultAsync(u => u.ID == user.ID);
            if (user_db != null)
            {
                if (!String.IsNullOrEmpty(user.Login) && !String.IsNullOrEmpty(user.Email))
                {
                    user_db.Login = user.Login;
                    user_db.Email = user.Email;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AccountPanel");
                }
            }
            ModelState.AddModelError("", "Некорректные данные");
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> Files()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            var files = await _context.Files.Where(f => f.CompanyID == comp_id).ToListAsync();
            return View(files);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                // путь к папке Files
                string path = "/files/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
                int user_id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                Models.File file = new Models.File
                {
                    CompanyID = comp_id,
                    UserID = user_id,
                    Name = uploadedFile.FileName,
                    Link = path,
                    CreationDate = DateTime.Now
                };

                _context.Files.Add(file);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Files");
        }

        [Authorize]
        [ActionName("DeleteFile")]
        public IActionResult ConfirmDeleteFile(int? id)
        {
            if (id != null)
            {
                Models.File file = _context.Files.FirstOrDefault(w => w.ID == id);
                if (file != null)
                {
                    return View(file);
                }
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteFile(int? id)
        {
            if (id != null)
            {
                Models.File file = await _context.Files.FirstOrDefaultAsync(f => f.ID == id);
                if (file != null)
                {
                    _context.Files.Remove(file);
                    await _context.SaveChangesAsync();
                    string path = "/files/" + file.Name;
                    if (System.IO.File.Exists(_appEnvironment.WebRootPath + path))
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + path);
                    }
                    return RedirectToAction("Files");
                }
            }
            return NotFound();
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
        public async Task<IActionResult> Workers()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            var workers = await _context.Workers.Where(x => x.CompanyID == comp_id).ToListAsync();
            return View(workers);
        }

        [Authorize]
        public IActionResult AddWorker()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> EditCompany(int? id)
        {
            if (id != null)
            {
                Company company = await _context.Companies.FirstOrDefaultAsync(c => c.ID == id);
                if (company != null)
                {
                    return View(company);
                }
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditCompany(Company company)
        {
            var company_db = await _context.Companies.FirstOrDefaultAsync(c => c.ID == company.ID);
            if (company_db != null)
            {
                if (!String.IsNullOrEmpty(company.Title) && !String.IsNullOrEmpty(company.WorkEmail))
                {
                    company_db.Title = company.Title;
                    company_db.WorkEmail = company.WorkEmail;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AdminPanel");
                }
                ModelState.AddModelError("", "Некорректные данные");
            }
            return View(company);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddWorker(Worker worker)
        {
            if (ModelState.IsValid)
            {
                int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
                worker.CompanyID = comp_id;
                _context.Workers.Add(worker);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPanel");
            }
            ModelState.AddModelError("", "Некорректные имя и/или фамилия");
            return View(worker);

        }
        [Authorize]
        public async Task<IActionResult> EditWorker(int? id)
        {
            if (id != null)
            {
                Worker worker = await  _context.Workers.FirstOrDefaultAsync(w => w.ID == id);
                if (worker != null)
                {
                    return View(worker);
                }
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditWorker(Worker worker)
        {
            if (ModelState.IsValid)
            {
                var db_worker = _context.Workers.FirstOrDefault(w => w.ID == worker.ID);
                if (db_worker != null)
                {
                    db_worker.FirstName = worker.FirstName;
                    db_worker.SecondName = worker.SecondName;
                    db_worker.Status = worker.Status;
                    db_worker.InviteCode = worker.InviteCode;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AdminPanel");
                }
                ModelState.AddModelError("", "Некорректные данные");
            }

            ModelState.AddModelError("", "Некорректные данные");
            return View(worker);
        }

        [HttpGet]
        [ActionName("DeleteWorker")]
        public IActionResult ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Worker worker =  _context.Workers.FirstOrDefault(w => w.ID == id);
                if (worker != null)
                {
                    return View(worker);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWorker(int? id)
        {
            if (id != null)
            {
                Worker worker = _context.Workers.FirstOrDefault(w => w.ID == id);
                if (worker != null)
                {
                    _context.Workers.Remove(worker);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AdminPanel");
                }
            }
            return NotFound();
        }
    }
}
