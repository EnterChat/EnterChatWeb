using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class Note
    {
        public int ID { get; set; }
        public int? UserID { get; set; }
        public int? CompanyID { get; set; }
        public int? NoteCategoryID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите тему заметки!")]
        [StringLength(20, ErrorMessage = "Навзвание заметки не может быть больше 20 символов!")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите текст заметки!")]
        [StringLength(250, ErrorMessage = "Текст заметки не может быть больше 250 символов!")]
        public string Text { get; set; }

        //public User User { get; set; }
        [NotMapped]
        public UserPlusWorkerModel UserPlusWorker { get; set; }
        public Company Company { get; set; }
        public NoteCategory NoteCategory { get; set; }
    }
}
