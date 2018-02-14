using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class Worker
    {
        public int ID { get; set; }
        public int CompanyID { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите своё имя!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите свою фамилию!")]
        public string SecondName { get; set; }
        public bool? Status { get; set; }
        public int InviteCode { get; set; }

        public Company Company { get; set; }
    }
}
