using EnterChatWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterChatWeb.Data
{
    public class EnterChatContext : DbContext
    {
        public EnterChatContext(DbContextOptions<EnterChatContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<TopicMessage> TopicMessages { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<GroupChatMessage> GroupChatMessages { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Worker> Workers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<TopicMessage>().ToTable("TopicMessage");
            modelBuilder.Entity<Topic>().ToTable("Topic");
            modelBuilder.Entity<Note>().ToTable("Note");
            modelBuilder.Entity<GroupChatMessage>().ToTable("GroupChatMessage");
            modelBuilder.Entity<File>().ToTable("File");
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Worker>().ToTable("Worker");


            base.OnModelCreating(modelBuilder);
        }
    }
}
