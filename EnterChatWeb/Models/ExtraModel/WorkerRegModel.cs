using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class WorkerRegModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите название вашей компании!")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Пожалуйста, придумайте логин!")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите адрес почты!")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Пожалуйста, придумайте пароль!")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите инвайт-код! Его можно получить " +
            "у администратора вашей компании")]
        public int InviteCode { get; set; }
    }
}
