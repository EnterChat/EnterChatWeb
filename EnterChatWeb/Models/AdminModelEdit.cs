using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class AdminModelEdit
    {
        public int ID { get; set; }
        public int CompanyID { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите имя работника!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите фамилию работника!")]
        public string SecondName { get; set; }
        public bool Status { get; set; }
    }
}
