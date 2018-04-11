using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class Department
    {
        public int ID { get; set; }
        public int? CompanyID { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите название отдела!")]
        public string Title { get; set; }
        public bool Status { get; set; }
        public string StringStatus
        {
            get
            {
                if (Status == true) return "Да";
                else return "Нет";
            }
        }

        public Company Company { get; set; }
        public ICollection<Worker> Workers { get; set; }
        public ICollection<UserPlusWorkerModel> UserPlusWorkers { get; set; }
    }
}
