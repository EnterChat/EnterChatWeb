using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models.ExtraModel
{
    public class GroupMessagePlusUser
    {
        public int UserID { get; set; }
        public int CompanyID { get; set; }
        public string DepartmentName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Email { get; set; }

        public ICollection<GroupChatMessage> GroupMessages { get; set; } 
    }
}
