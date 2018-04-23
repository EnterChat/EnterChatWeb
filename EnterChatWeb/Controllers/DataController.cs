using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EnterChatWeb.Data;
using EnterChatWeb.Models;
using EnterChatWeb.Models.ExtraModel;
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

        //public DataController(EnterChatContext context, IHostingEnvironment hostingEnvironment)
        //{
        //    _context = context;
        //    _appEnvironment = hostingEnvironment;
        //}

        public DataController(EnterChatContext context)
        {
            _context = context;
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
            var deps = await _context.Departments.Where(d => d.CompanyID == comp_id).ToListAsync();
            //var workers = await _context.Workers.Where(x => x.CompanyID == comp_id).ToListAsync();
            foreach (Department dep in deps)
            {
                var workers = await _context.Workers.Where(x => x.DepartmentID == dep.ID).ToListAsync();
                List<UserPlusWorkerModel> userPlusWorkers = new List<UserPlusWorkerModel>();
                if (workers != null)
                {
                    foreach (Worker w in workers)
                    {
                        UserPlusWorkerModel userPlusWorkerModel = new UserPlusWorkerModel(w.FirstName, w.SecondName, w.InviteCode);
                        User user = await _context.Users.Where(u => u.WorkerID == w.ID).FirstOrDefaultAsync();
                        if (user != null)
                        {
                            userPlusWorkerModel.RegistrationDate = user.RegistrationDate;
                            userPlusWorkerModel.Email = user.Email;
                        }
                        userPlusWorkers.Add(userPlusWorkerModel);
                    }
                    dep.UserPlusWorkers = userPlusWorkers;
                }

            }
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ID == comp_id);
            /*AdminPanelModel model = new AdminPanelModel();
            model.Company = company;
            model.Workers = workers;*/
            company.Departments = deps;
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
            foreach (Models.File f in  files)
            {
                User user = await _context.Users.Where(w => w.ID == f.UserID).FirstOrDefaultAsync();
                Worker worker = await _context.Workers.Where(w => w.ID == user.WorkerID).FirstOrDefaultAsync();
                Department department = await _context.Departments.Where(d => d.ID == worker.DepartmentID).FirstOrDefaultAsync();
                UserPlusWorkerModel model = new UserPlusWorkerModel(worker.FirstName, worker.SecondName,
                    user.Email, department.Title);
                f.UserPlusWorker = model;
            }
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
        public async Task<IActionResult> GroupChat()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            int user_id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var messages = await _context.GroupChatMessages.Where(m => m.CompanyID == comp_id).ToListAsync();
            User user = await _context.Users.Where(u => u.ID == user_id).FirstOrDefaultAsync();
            Worker worker = await _context.Workers.Where(w => w.ID == user.WorkerID).FirstOrDefaultAsync();
            Department department = await _context.Departments.Where(d => d.ID == worker.DepartmentID).FirstOrDefaultAsync();

            foreach (GroupChatMessage message in messages)
            {
                User _user = await _context.Users.Where(u => u.ID == message.UserID).FirstOrDefaultAsync();
                Worker _worker = await _context.Workers.Where(w => w.ID == user.WorkerID).FirstOrDefaultAsync();
                Department _department = await _context.Departments.Where(d => d.ID == worker.DepartmentID)
                    .FirstOrDefaultAsync();
                UserPlusWorkerModel _model = new UserPlusWorkerModel(_worker.FirstName, _worker.SecondName,
                    _user.Email, _department.Title);
                message.UserPlusWorker = _model;
            }

            GroupMessagePlusUser model = new GroupMessagePlusUser
            {
                UserID = user_id,
                CompanyID = comp_id,
                Email = user.Email,
                FirstName = worker.FirstName,
                SecondName = worker.SecondName,
                DepartmentName = department.Title,
                GroupMessages = messages
            };

            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> Notes()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            var notes = await _context.Notes.Where(n => n.CompanyID == comp_id).ToListAsync();
            foreach (Note n in notes)
            {
                User user = await _context.Users.Where(w => w.ID == n.UserID).FirstOrDefaultAsync();
                Worker worker = await _context.Workers.Where(w => w.ID == user.WorkerID).FirstOrDefaultAsync();
                Department department = await _context.Departments.Where(d => d.ID == worker.DepartmentID).FirstOrDefaultAsync();
                UserPlusWorkerModel model = new UserPlusWorkerModel(worker.FirstName, worker.SecondName,
                    user.Email, department.Title);
                n.UserPlusWorker = model;
            }
            return View(notes);
        }

        [Authorize]
        public IActionResult AddNote()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNote(Note note)
        {
            if (ModelState.IsValid)
            {
                int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
                int user_id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                note.CompanyID = comp_id;
                note.UserID = user_id;
                note.CreationDate = DateTime.Now;
                _context.Notes.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction("Notes");
            }
            ModelState.AddModelError("", "Некорректные данные");
            return View(note);
        }

        [Authorize]
        public async Task<IActionResult> EditNote(int? id)
        {
            if (id != null)
            {
                Note note = await _context.Notes.FirstOrDefaultAsync(n => n.ID == id);
                if (note != null)
                {
                    return View(note);
                }
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditNote(Note note)
        {
            if (ModelState.IsValid)
            {
                var db_note = await _context.Notes.FirstOrDefaultAsync(n => n.ID == note.ID);
                if (db_note != null)
                {
                    db_note.Title = note.Title;
                    db_note.Text = note.Text;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Notes");
                }
            }
            ModelState.AddModelError("", "Некорректные данные");
            return View(note);
        }

        [HttpGet]
        [ActionName("DeleteNote")]
        public IActionResult ConfirmDeleteNote(int? id)
        {
            if (id != null)
            {
                Note note = _context.Notes.FirstOrDefault(n => n.ID == id);
                if (note != null)
                {
                    return View(note);
                }
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteNote(int? id)
        {
            if (id != null)
            {
                Note note = await _context.Notes.FirstOrDefaultAsync(n => n.ID == id);
                if (note != null)
                {
                    _context.Notes.Remove(note);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Notes");
                }
            }
            return NotFound();
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
            //var workers = await _context.Workers.Where(x => x.CompanyID == comp_id).ToListAsync();
            var departments = await _context.Departments.Where(d => d.CompanyID == comp_id).ToListAsync();
            foreach (Department dep in departments)
            {
                var workers = await _context.Workers.Where(x => x.DepartmentID == dep.ID).ToListAsync();
                List<UserPlusWorkerModel> userPlusWorkers = new List<UserPlusWorkerModel>();
                if (workers != null)
                {
                    foreach (Worker w in workers)
                    {
                        UserPlusWorkerModel userPlusWorkerModel = new UserPlusWorkerModel(w.FirstName, w.SecondName, w.InviteCode);
                        User user = await _context.Users.Where(u => u.WorkerID == w.ID).FirstOrDefaultAsync();
                        if (user != null)
                        {
                            userPlusWorkerModel.RegistrationDate = user.RegistrationDate;
                            userPlusWorkerModel.Email = user.Email;
                        }
                        userPlusWorkers.Add(userPlusWorkerModel);
                    }
                    dep.UserPlusWorkers = userPlusWorkers;
                }
            }
            return View(departments);
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
        public async Task<IActionResult> AddWorker()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            var departments = await _context.Departments.Where(d => d.CompanyID == comp_id).ToListAsync();
            WorkerPlusDepsList list = new WorkerPlusDepsList();
            list.Departments = departments;
            return View(list);
        }

        [Authorize]
        public ActionResult AddDep()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddDep(Department department)
        {
            if (ModelState.IsValid)
            {
                int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
                department.CompanyID = comp_id;
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPanel");
            }
            ModelState.AddModelError("", "Некорректные данные");
            return View(department);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddWorker(WorkerPlusDepsList workerModel)
        {
            if (ModelState.IsValid)
            {
                int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
                Worker worker = new Worker
                {
                    CompanyID = comp_id,
                    FirstName = workerModel.FirstName,
                    SecondName = workerModel.SecondName,
                    DepartmentID = workerModel.DepartmentID,
                    InviteCode = workerModel.InviteCode
                };
                
                _context.Workers.Add(worker);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPanel");
            }
            ModelState.AddModelError("", "Некорректные имя и/или фамилия");
            return View(workerModel);

        }
        [Authorize]
        public async Task<IActionResult> EditWorker(int? id)
        {
            if (id != null)
            {
                Worker worker = await  _context.Workers.FirstOrDefaultAsync(w => w.ID == id);
                if (worker != null)
                {
                    var departments = await _context.Departments.Where(d => d.CompanyID == worker.CompanyID).ToListAsync();
                    WorkerPlusDepsList model = new WorkerPlusDepsList
                    {
                        FirstName = worker.FirstName,
                        SecondName = worker.SecondName,
                        InviteCode = worker.InviteCode,
                        DepartmentID = worker.DepartmentID,
                        ID = worker.ID,
                        Departments = departments
                    };
                    return View(model);
                }
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditWorker(WorkerPlusDepsList model)
        {
            if (ModelState.IsValid)
            {
                var db_worker = _context.Workers.FirstOrDefault(w => w.ID == model.ID);
                if (db_worker != null)
                {
                    db_worker.FirstName = model.FirstName;
                    db_worker.SecondName = model.SecondName;
                    db_worker.DepartmentID = model.DepartmentID;
                    db_worker.InviteCode = model.InviteCode;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AdminPanel");
                }
                ModelState.AddModelError("", "Некорректные данные");
            }

            ModelState.AddModelError("", "Некорректные данные");
            return View(model);
        }

        public async void AddMessage(string inputMessage)
        {
            var user = inputMessage.Split(':')[0];
            var message = inputMessage.Skip(user.Length);
            var userModel = await _context.Users.FirstOrDefaultAsync(x => x.Login ==  message);
            var companyModel =await _context.Companies.FirstOrDefaultAsync(x => x.Users == userModel);
            GroupChatMessage newMessage = new GroupChatMessage
            {
                UserID = userModel.ID,
                CompanyID = companyModel.ID,
                Text = inputMessage,
                CreationDate = DateTime.Now
            };
            await _context.GroupChatMessages.AddAsync(newMessage);
            await _context.SaveChangesAsync();
        }
        [Authorize]
        public async Task<IActionResult> EditAdmin(int? id)
        {
            if (id != null)
            {
                Worker worker = await _context.Workers.FirstOrDefaultAsync(w => w.ID == id);


                if (worker != null)
                {
                    AdminModelEdit adminModelEdit = new AdminModelEdit
                    {
                        ID = worker.ID,
                        CompanyID = worker.CompanyID,
                        FirstName = worker.FirstName,
                        SecondName = worker.SecondName,
                        //Status = worker.Status
                    };

                    return View(adminModelEdit);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditAdmin(AdminModelEdit model)
        {
            if (ModelState.IsValid)
            {
                var db_worker = _context.Workers.FirstOrDefault(w => w.ID == model.ID);
                if (db_worker != null)
                {
                    db_worker.FirstName = model.FirstName;
                    db_worker.SecondName = model.SecondName;
                    //db_worker.Status = model.Status;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AdminPanel");
                }
                ModelState.AddModelError("", "Некорректные данные");
            }

            ModelState.AddModelError("", "Некорректные данные");
            return View(model);
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
