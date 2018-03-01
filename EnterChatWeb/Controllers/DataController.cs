using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EnterChatWeb.Data;
using EnterChatWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnterChatWeb.Controllers
{
    public class DataController : Controller
    {
        private EnterChatContext _context;

        public DataController(EnterChatContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult About()
        {
            string user_id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            return View();
        }

        [Authorize(Policy = "OnlyForAdmin")]
        public async Task<IActionResult> AdminPanel()
        {
            int comp_id = Int32.Parse(HttpContext.User.FindFirst("CompanyID").Value);
            var workers = await _context.Workers.Where(x => x.CompanyID == comp_id).ToListAsync();
            return View(workers);
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

        [Authorize]
        public IActionResult AddWorker()
        {
            return View();
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
        public async Task<IActionResult> Edit(int? id)
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
        public async Task<IActionResult> Edit(Worker worker)
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
        [ActionName("Delete")]
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
        public async Task<IActionResult> Delete(int? id)
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
