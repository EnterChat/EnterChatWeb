using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class Worker
    {
        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public bool? Status { get; set; }
        public int InviteCode { get; set; }

        public Company Company { get; set; }
    }
}
