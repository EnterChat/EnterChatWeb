using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models.ExtraModel
{
    public class TopicPlusWorkersList
    {
        [Required(ErrorMessage = "Пожалуйста, введите название чата!")]
        public string Title { get; set; }
        public List<WorkerChatMember> WorkerChatMembers { get; set; }
    }
}
