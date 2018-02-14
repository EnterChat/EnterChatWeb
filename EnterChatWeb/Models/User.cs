using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class User
    {
        public int ID { get; set; }
        public int WorkerID { get; set; }
        public int CompanyID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }
        [Required(ErrorMessage = "Пожалуйста, придумайте логин!")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите адрес вашей почты!")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Пожалуйста, придумайте пароль!")]
        public string Password { get; set; }

        public Company Company { get; set; }
        public Worker Worker { get; set; }
    }
}
