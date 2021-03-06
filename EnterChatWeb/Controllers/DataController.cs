﻿using System;
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
            company.Departments = departments;
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
                        UserPlusWorkerModel userPlusWorkerModel = new UserPlusWorkerModel(w.FirstName, w.SecondName, w.InviteCode, w.ID);
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
                UserPlusWorkerModel model = new UserPlusWorkerModel(worker.FirstName, worker.SecondName,
                    user.Email);
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
                Worker _worker = await _context.Workers.Where(w => w.ID == _user.WorkerID).FirstOrDefaultAsync();
                UserPlusWorkerModel _model = new UserPlusWorkerModel(_worker.FirstName, _worker.SecondName,
                    _user.Email);
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
            var noteCategories = await _context.NoteCategories.Where(nc => nc.CompanyID == comp_id).ToListAsync();
            foreach (NoteCategory noteCategory in noteCategories)
            {
                var notes = await _context.Notes.Where(n => n.NoteCategoryID == noteCategory.ID).ToListAsync();
                foreach (Note note in notes)
                {
                    User user = await _context.Users.Where(w => w.ID == note.UserID).FirstOrDefaultAsync();
                    Worker worker = await _context.Workers.Where(w => w.ID == user.WorkerID).FirstOrDefaultAsync();
                    UserPlusWorkerModel model = new UserPlusWorkerModel(worker.FirstName, worker.SecondName,
                        user.Email);
                    note.UserPlusWorker = model;
                }
                noteCategory.Notes = notes;
            }
            return View(noteCategories);
        }

        [Authorize]
        public IActionResult AddNoteCategory()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNoteCategory(NoteCategory noteCategory)
        {
            if (ModelState.IsValid)
            {
                int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
                NoteCategory noteCategoryDB = new NoteCategory
                {
                    CompanyID = comp_id,
                    Title = noteCategory.Title
                };
                await _context.AddAsync(noteCategoryDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("Notes");
            }
            ModelState.AddModelError("", "Некорректные данные");
            return View(noteCategory);
        }

        [Authorize]
        public async Task<IActionResult> AddNote()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            var noteCategories = await _context.NoteCategories.Where(n => n.CompanyID == comp_id).ToListAsync();
            NotePlusCategoriesList notePlusCategories = new NotePlusCategoriesList();
            notePlusCategories.NoteCategories = noteCategories;
            return View(notePlusCategories);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNote(NotePlusCategoriesList noteplusList)
        {
            if (ModelState.IsValid)
            {
                int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
                int user_id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                Note note = new Note
                {
                    CompanyID = comp_id,
                    UserID = user_id,
                    CreationDate = DateTime.Now,
                    NoteCategoryID = noteplusList.NoteCategoryID,
                    Title = noteplusList.Title,
                    Text = noteplusList.Text
                };
                
                _context.Notes.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction("Notes");
            }
            ModelState.AddModelError("", "Некорректные данные");
            return View(noteplusList);
        }

        [Authorize]
        public async Task<IActionResult> EditNote(int? id)
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            if (id != null)
            {
                Note note = await _context.Notes.FirstOrDefaultAsync(n => n.ID == id);
                if (note != null)
                {
                    var noteCategories = await _context.NoteCategories.Where(n => n.CompanyID == comp_id).ToListAsync();
                    NotePlusCategoriesList notePlus = new NotePlusCategoriesList
                    {
                        Text = note.Text,
                        Title = note.Title,
                        ID = note.ID
                    };
                    notePlus.NoteCategories = noteCategories;
                    return View(notePlus);
                }
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditNote(NotePlusCategoriesList note)
        {
            if (ModelState.IsValid)
            {
                var db_note = await _context.Notes.FirstOrDefaultAsync(n => n.ID == note.ID);
                if (db_note != null)
                {
                    db_note.Title = note.Title;
                    db_note.Text = note.Text;
                    db_note.NoteCategoryID = note.NoteCategoryID;
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
        public async Task<IActionResult> Topics()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            int user_id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            User user = await _context.Users.Where(u => u.ID == user_id).FirstOrDefaultAsync();
            int? w_ID = user.WorkerID;
            List<Topic> topics = new List<Topic>();
            var chatmemberships = await _context.ChatMembers.Where(c => c.WorkerID == w_ID).ToListAsync();

            foreach (ChatMember ch in chatmemberships)
            {
                Topic topic = await _context.Topics.Where(t => t.ID == ch.TopicID).FirstOrDefaultAsync();
                var dischatmembers = await _context.ChatMembers.Where(c => c.TopicID == topic.ID).ToListAsync();
                List<WorkerChatMember> workerChatMembers = new List<WorkerChatMember>();
                foreach (ChatMember dch in dischatmembers)
                {
                    Worker worker = await _context.Workers.Where(w => w.ID == dch.WorkerID).FirstOrDefaultAsync();
                    if (worker != null)
                    {
                        WorkerChatMember dischatMember = new WorkerChatMember
                        {
                            FullName = worker.FirstName + " " + worker.SecondName
                        };
                        workerChatMembers.Add(dischatMember);
                    }
                }
                topic.WorkerChatMembers = workerChatMembers;
                if (topics != null) topics.Add(topic);
            }
            return View(topics);
        }

        [Authorize]
        public async Task<IActionResult> AddTopic()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            int user_id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.Where(u => u.ID == user_id).FirstOrDefaultAsync();
            int? w_id = user.WorkerID;
            var workers = await _context.Workers.Where(w => w.CompanyID == comp_id).ToListAsync();
            List<WorkerChatMember> workerChatMembers = new List<WorkerChatMember>();
            foreach (Worker worker in workers)
            {
                if (worker.ID != w_id)
                {
                    WorkerChatMember member = new WorkerChatMember
                    {
                        FullName = worker.FirstName + " " + worker.SecondName,
                        ID = worker.ID,
                        IsAdded = false
                    };
                    workerChatMembers.Add(member);
                }
            }

            TopicPlusWorkersList topicPlusWorkersList = new TopicPlusWorkersList
            {
                WorkerChatMembers = workerChatMembers
            };

            return View(topicPlusWorkersList);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTopic(TopicPlusWorkersList list)
        {
            if (ModelState.IsValid)
            {
                int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
                int user_id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                User user = await _context.Users.Where(u => u.ID == user_id).FirstOrDefaultAsync();
                int? w_ID = user.WorkerID;

                Topic topic = new Topic
                {
                    UserID = user_id,
                    CompanyID = comp_id,
                    Title = list.Title,
                    CreationDate = DateTime.Now
                };

                await _context.Topics.AddAsync(topic);
                await _context.SaveChangesAsync();

                List<ChatMember> chatMembers = new List<ChatMember>();
                ChatMember chAdmin = new ChatMember
                {
                    WorkerID = w_ID,
                    TopicID = topic.ID
                };

                await _context.ChatMembers.AddAsync(chAdmin);
                await _context.SaveChangesAsync();

                foreach (WorkerChatMember member in list.WorkerChatMembers){
                    if (member.IsAdded == true)
                    {
                        ChatMember chmember = new ChatMember
                        {
                            WorkerID = member.ID,
                            TopicID = topic.ID
                        };
                        chatMembers.Add(chmember);
                    }
                }

                await _context.ChatMembers.AddRangeAsync(chatMembers);
                await _context.SaveChangesAsync();

                return RedirectToAction("Topics");
            }
            ModelState.AddModelError("", "Некорректные данные");
            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> DeleteChatMember(int? id)
        {
            if (id != null)
            {
                EditChatMemberExtraModel model = new EditChatMemberExtraModel();
                List<WorkerChatMember> listMembers = new List<WorkerChatMember>();
                Topic topic = await _context.Topics.Where(t => t.ID == id).FirstOrDefaultAsync();
                var chatmembers = await _context.ChatMembers.Where(ch => ch.TopicID == id).ToListAsync();
                foreach (ChatMember mem in chatmembers)
                {
                    Worker worker = await _context.Workers.Where(w => w.ID == mem.WorkerID).FirstOrDefaultAsync();
                    if(worker != null)
                    {
                        WorkerChatMember member = new WorkerChatMember
                        {
                            FullName = worker.FirstName + " " + worker.SecondName,
                            ID = worker.ID
                        };
                        listMembers.Add(member);
                    }
                }
                model.Title = topic.Title;
                model.TopicID = topic.ID;
                model.WorkerChatMembers = listMembers;
                return View(model);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteChatMember(EditChatMemberExtraModel model) //неправильно
        {
            if (ModelState.IsValid)
            {
                ChatMember member = await _context.ChatMembers.Where(ch => ch.WorkerID == model.ID &&
                ch.TopicID == model.TopicID).FirstOrDefaultAsync();
                if(member != null)
                {
                    _context.ChatMembers.Remove(member);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Topics");
                }
            }
            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> AddChatMember(int? id)
        {
            if (id != null)
            {
                int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
                EditChatMemberExtraModel model = new EditChatMemberExtraModel();
                List<WorkerChatMember> listMembers = new List<WorkerChatMember>();
                Topic topic = await _context.Topics.Where(t => t.ID == id).FirstOrDefaultAsync();

                var chatmembers = await _context.ChatMembers.Where(ch => ch.TopicID == id).ToListAsync();

                var workers = await _context.Workers.Where(w => w.CompanyID == comp_id).ToListAsync();

                foreach (Worker worker in workers)
                {
                    ChatMember member = chatmembers.Find(w => w.WorkerID == worker.ID);
                    if (member == null)
                    {
                        WorkerChatMember wmember = new WorkerChatMember
                        {
                            FullName = worker.FirstName + " " + worker.SecondName,
                            ID = worker.ID
                        };
                        listMembers.Add(wmember);
                    }
                }

                model.Title = topic.Title;
                model.TopicID = topic.ID;
                model.WorkerChatMembers = listMembers;
                return View(model);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddChatMember(EditChatMemberExtraModel model)
        {
            if (ModelState.IsValid)
            {
                ChatMember member = new ChatMember()
                {
                    WorkerID = model.ID,
                    TopicID = model.TopicID
                };

                await _context.ChatMembers.AddAsync(member);
                await _context.SaveChangesAsync();
                return RedirectToAction("Topics");
            }
            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> EditTopicTitle(int? id)
        {
            if (id != null)
            {
                Topic topic = await _context.Topics.Where(t => t.ID == id).FirstOrDefaultAsync();
                if (topic != null)
                {
                    return View(topic);
                }
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditTopicTitle(Topic topic)
        {
            if (ModelState.IsValid)
            {
                var topic_db = await _context.Topics.Where(t => t.ID == topic.ID).FirstOrDefaultAsync();
                if (topic_db != null) topic_db.Title = topic.Title;
                await _context.SaveChangesAsync();
                return RedirectToAction("Topics");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
            }
            return View(topic);
        }

        [Authorize]
        [ActionName("DeleteTopic")]
        public async Task<IActionResult> ConfirmDeleteTopic(int? id)
        {
            if (id != null)
            {
                Topic topic = await _context.Topics.Where(t => t.ID == id).FirstOrDefaultAsync();
                if (topic != null) return View(topic);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteTopic(int? id)
        {
            if (id != null)
            {
                Topic topic = await _context.Topics.Where(t => t.ID == id).FirstOrDefaultAsync();
                var topicMessages = await _context.TopicMessages.Where(tm => tm.TopicID == id).ToListAsync();
                var chatMembers = await _context.ChatMembers.Where(ch => ch.TopicID == id).ToListAsync();
                if (topic != null && topicMessages != null && chatMembers != null) {
                    _context.Topics.Remove(topic);
                    _context.TopicMessages.RemoveRange(topicMessages);
                    _context.ChatMembers.RemoveRange(chatMembers);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Topics");
                }
            }
            return NotFound();
        }

        [Authorize]
        [ActionName("LeaveTopicChat")]
        public async Task<IActionResult> ConfirmLeaveTopicChat(int? id)
        {
            if (id != null)
            {
                Topic topic = await _context.Topics.Where(t => t.ID == id).FirstOrDefaultAsync();
                if (topic != null) return View(topic);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LeaveTopicChat(int? id)
        {
            if (id != null)
            {
                int user_id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                User user = await _context.Users.Where(u => u.ID == user_id).FirstOrDefaultAsync();
                Worker worker = await _context.Workers.Where(w => w.ID == user.WorkerID).FirstOrDefaultAsync();
                ChatMember member = await _context.ChatMembers.Where(ch => ch.TopicID == id &&
                ch.WorkerID == worker.ID).FirstOrDefaultAsync();
                if (member != null)
                {
                    _context.ChatMembers.Remove(member);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Topics");
                }
                else
                {
                    return NotFound();
                }
            }
            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> TopicChat(int? id)
        {
            if (id != null)
            {
                Topic topic = await _context.Topics.Where(t => t.ID == id).FirstOrDefaultAsync();
                if (topic != null)
                {
                    int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
                    int user_id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    User user = await _context.Users.Where(u => u.ID == user_id).FirstOrDefaultAsync();
                    Worker worker = await _context.Workers.Where(w => w.ID == user.WorkerID).FirstOrDefaultAsync();
                    TopicMessagePlusUser topicMessagePlusUser = new TopicMessagePlusUser
                    {
                        CompanyID = comp_id,
                        UserID = user_id,
                        TopicID = topic.ID,
                        FirstName = worker.FirstName,
                        SecondName = worker.SecondName,
                        TopicTitle = topic.Title
                    };
                    var TopicMessages = await _context.TopicMessages.Where(t => t.TopicID == topic.ID).ToListAsync();
                    foreach (var topicmes in TopicMessages)
                    {
                        User _user = await _context.Users.Where(u => u.ID == topicmes.UserID).FirstOrDefaultAsync();
                        Worker _worker = await _context.Workers.Where(w => w.ID == _user.WorkerID).FirstOrDefaultAsync();
                        UserPlusWorkerModel userPlusWorkerModel = 
                            new UserPlusWorkerModel(_worker.FirstName, _worker.SecondName, _user.Email);
                        topicmes.UserPlusWorker = userPlusWorkerModel;
                    }
                    topicMessagePlusUser.TopicMessages = TopicMessages;
                    return View(topicMessagePlusUser);
                }
            }
            return NotFound();
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
