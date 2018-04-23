using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models.ExtraModel
{
    public class UserPlusText
    {
        public int UserID { get; set; }
        public int CompanyID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Text { get; set; }
    }
}
