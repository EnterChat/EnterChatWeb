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
        [StringLength(20, ErrorMessage = "Логин не может быть больше 20 символов!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите пароль!")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Пароль не может быть больше 20 символов!")]
        public string Password { get; set; }

    }
}
