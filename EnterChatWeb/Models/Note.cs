using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class Note
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int CompanyID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public User User { get; set; }
        public Company Company { get; set; }
    }
}
