﻿using EnterChatWeb.Models.ExtraModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Models
{
    public class Topic
    {
        public int ID { get; set; }
        public int? CompanyID { get; set; }
        public int? UserID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }
        public string Title { get; set; }

        public ICollection<TopicMessage> TopicMessages { get; set; }
        [NotMapped]
        public ICollection<WorkerChatMember> WorkerChatMembers { get; set; }
        public Company Company { get; set; }
        public User User { get; set; }
    }
}
