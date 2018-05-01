using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class NoteCategory
    {
        public int ID { get; set; }
        public int? CompanyID { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите название категории!")]
        public string Title { get; set; }

        public Company Company { get; set; }
        public ICollection<Note> Notes { get; set; }
    }
}
