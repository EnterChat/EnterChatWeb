using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class AdminPanelModel
    {
        public Company Company {get; set; }
        public ICollection<Worker> Workers { get; set; }
    }
}
