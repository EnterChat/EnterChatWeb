using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class Company
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название компании!")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите адрес корпоративной почты!")]
        [EmailAddress]
        public string WorkEmail { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Topic> Topics { get; set; }
        public ICollection<Note> Notes { get; set; }
        public ICollection<File> Files { get; set; }
        public ICollection<Worker> Workers { get; set; }
        public ICollection<GroupChatMessage> GroupChatMessages { get; set; }
    }
}
