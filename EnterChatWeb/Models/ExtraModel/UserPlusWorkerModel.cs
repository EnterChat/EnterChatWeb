using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class UserPlusWorkerModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Email { get; set; }
        public int? InviteCode { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RegistrationDate { get; set; }

        /*public UserPlusWorkerModel(string f, string s, string e, int? i, DateTime? d)
        {
            FirstName = f;
            SecondName = s;
            Email = e;
            InviteCode = i;
            RegistrationDate = d;
        }*/

        public UserPlusWorkerModel(string f, string s, int? i)
        {
            FirstName = f;
            SecondName = s;
            InviteCode = i;
        }
    }
}
