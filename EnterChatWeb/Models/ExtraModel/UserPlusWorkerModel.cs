using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class UserPlusWorkerModel
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public bool Status { get; set; }
        public string StringStatus
        {
            get
            {
                if (Status == true) return "Да";
                else return "Нет";
            }
        }
        public string Email { get; set; }
    }
}
