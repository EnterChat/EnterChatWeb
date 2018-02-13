using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class User
    {
        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public bool? Status { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }
        public string Login { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

        public Company Company { get; set; }
    }
}
