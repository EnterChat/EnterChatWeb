using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models.ExtraModel
{
    public class WorkerChatMember
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public bool IsAdded { get; set; }
    }
}
