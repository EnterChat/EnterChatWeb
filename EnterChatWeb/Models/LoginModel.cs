using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите логин!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите пароль!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
