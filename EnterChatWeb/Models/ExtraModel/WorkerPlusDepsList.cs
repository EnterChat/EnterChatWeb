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
        public int? ID { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите имя работника!")]
        [StringLength(20, ErrorMessage = "Имя не может быть больше 20 символов!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите фамилию работника!")]
        [StringLength(20, ErrorMessage = "Фамилия не может быть больше 20 символов!")]
        public string SecondName { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите пригласительный код для работника!")]
        [Range(10, 9999, ErrorMessage = "Пожалуйста, введите именно число от 10 до 9999!")]
        public int? InviteCode { get; set; }
    }
}
