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
        [StringLength(20, ErrorMessage = "Название компании не может быть больше 20 символов!")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Пожалуйста, придумайте логин!")]
        [StringLength(20, ErrorMessage = "Логин не может быть больше 20 символов!")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите адрес почты!")]
        [EmailAddress]
        [StringLength(20, ErrorMessage = "Адрес почты не может быть больше 20 символов!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Пожалуйста, придумайте пароль!")]
        [Range(8, 20, ErrorMessage = "Пароль должен быть не меньше 8 и не больше 20 символов!")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите инвайт-код! Его можно получить " +
            "у администратора вашей компании")]
        [Range(10, 9999, ErrorMessage = "Пожалуйста, введите именно число от 10 до 9999!")]
        public int InviteCode { get; set; }
    }
}
