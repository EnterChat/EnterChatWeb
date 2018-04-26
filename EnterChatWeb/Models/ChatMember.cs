using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class ChatMember
    {
        public int ID { get; set; }
        public int? TopicID { get; set; }
        public int? WorkerID { get; set; }
    }
}
