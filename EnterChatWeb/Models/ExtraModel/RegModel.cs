using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class RegModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите название компании!")]
        [StringLength(20, ErrorMessage = "Название не может быть больше 20 символов!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите адрес корпоративной почты!")]
        [StringLength(30, ErrorMessage = "Адрес почты не может быть больше 30 символов!")]
        [EmailAddress]
        public string WorkEmail { get; set; }

        [Required(ErrorMessage = "Пожалуйста, придумайте логин!")]
        [StringLength(20, ErrorMessage = "Логин не может быть больше 20 символов!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите адрес вашей почты!")]
        [StringLength(30, ErrorMessage = "Адрес почты не может быть больше 20 символов!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пожалуйста, придумайте пароль!")]
        [Range(8, 20, ErrorMessage = "Пароль должен быть не меньше 8 и не больше 20 символов!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название первого отдела!")]
        [StringLength(20, ErrorMessage = "Название отдела не может быть больше 20 символов!")]
        public string DepTitle { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите своё имя!")]
        [StringLength(20, ErrorMessage = "Имя не может быть больше 20 символов!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите свою фамилию!")]
        [StringLength(20, ErrorMessage = "Фамилия не может быть больше 20 символов!")]
        public string SecondName { get; set; }
    }
}
