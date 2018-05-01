using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models.ExtraModel
{
    public class NotePlusCategoriesList
    {
        public ICollection<NoteCategory> NoteCategories { get; set; }
        public int? NoteCategoryID { get; set; }
        public int? ID { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите тему заметки!")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите текст заметки!")]
        public string Text { get; set; }
    }
}
