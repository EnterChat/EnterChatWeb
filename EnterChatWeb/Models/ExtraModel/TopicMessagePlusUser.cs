using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models.ExtraModel
{
    public class TopicMessagePlusUser
    {
        public int? TopicID { get; set; }
        public int? UserID { get; set; }
        public int? CompanyID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public ICollection<TopicMessage> TopicMessages { get; set; }
    }
}
