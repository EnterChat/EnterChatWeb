using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models.ExtraModel
{
    public class EditChatMemberExtraModel
    {
        public int? ID { get; set; }
        public string Title { get; set; }
        public IEnumerable<WorkerChatMember> WorkerChatMembers { get; set; }
    }
}
