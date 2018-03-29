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
        public int? CompanyID { get; set; }
        //public int UserID { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите имя работника!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите фамилию работника!")]
        public string SecondName { get; set; }
        public bool Status { get; set; }
        public string StringStatus
        {
            get
            {
                if (Status == true) return "Да";
                else return "Нет";
            }
        }
        [Required(ErrorMessage = "Пожалуйста, введите пригласительный код для работника!")]
        public int? InviteCode { get; set; }

        public Company Company { get; set; }
        //public User User { get; set; }
    }
}
