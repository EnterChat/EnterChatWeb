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
        public string Title { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите адрес корпоративной почты!")]
        [EmailAddress]
        public string WorkEmail { get; set; }

        [Required(ErrorMessage = "Пожалуйста, придумайте логин!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите адрес вашей почты!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пожалуйста, придумайте пароль!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название первого отдела!")]
        public string DepTitle { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите своё имя!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите свою фамилию!")]
        public string SecondName { get; set; }
    }
}
