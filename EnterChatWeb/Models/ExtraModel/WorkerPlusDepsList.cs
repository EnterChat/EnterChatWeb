using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models.ExtraModel
{
    public class WorkerPlusDepsList
    {
        public ICollection<Department> Departments;
        public int? DepartmentID { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите имя работника!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите фамилию работника!")]
        public string SecondName { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите пригласительный код для работника!")]
        public int? InviteCode { get; set; }
    }
}
