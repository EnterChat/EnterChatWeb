﻿// <auto-generated />
using EnterChatWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace EnterChatWeb.Migrations
{
    [DbContext(typeof(EnterChatContext))]
    partial class EnterChatContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EnterChatWeb.Models.Company", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Title");

                    b.Property<string>("WorkEmail");

                    b.HasKey("ID");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("EnterChatWeb.Models.File", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompanyID");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Link");

                    b.Property<string>("Name");

                    b.Property<int?>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("CompanyID");

                    b.HasIndex("UserID");

                    b.ToTable("File");
                });

            modelBuilder.Entity("EnterChatWeb.Models.GroupChatMessage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CompanyID");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Text");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("CompanyID");

                    b.HasIndex("UserID");

                    b.ToTable("GroupChatMessage");
                });

            modelBuilder.Entity("EnterChatWeb.Models.Note", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompanyID");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int?>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("CompanyID");

                    b.HasIndex("UserID");

                    b.ToTable("Note");
                });

            modelBuilder.Entity("EnterChatWeb.Models.Topic", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompanyID");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Title");

                    b.Property<int?>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("CompanyID");

                    b.HasIndex("UserID");

                    b.ToTable("Topic");
                });

            modelBuilder.Entity("EnterChatWeb.Models.TopicMessage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompanyID");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Text");

                    b.Property<int?>("TopicID");

                    b.Property<int?>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("CompanyID");

                    b.HasIndex("TopicID");

                    b.HasIndex("UserID");

                    b.ToTable("TopicMessage");
                });

            modelBuilder.Entity("EnterChatWeb.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompanyID");

                    b.Property<string>("Email");

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<int?>("WorkerID");

                    b.HasKey("ID");

                    b.HasIndex("CompanyID");

                    b.HasIndex("WorkerID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("EnterChatWeb.Models.Worker", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompanyID");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<int?>("InviteCode")
                        .IsRequired();

                    b.Property<string>("SecondName")
                        .IsRequired();

                    b.Property<bool>("Status");

                    b.HasKey("ID");

                    b.HasIndex("CompanyID");

                    b.ToTable("Worker");
                });

            modelBuilder.Entity("EnterChatWeb.Models.File", b =>
                {
                    b.HasOne("EnterChatWeb.Models.Company", "Company")
                        .WithMany("Files")
                        .HasForeignKey("CompanyID");

                    b.HasOne("EnterChatWeb.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("EnterChatWeb.Models.GroupChatMessage", b =>
                {
                    b.HasOne("EnterChatWeb.Models.Company", "Company")
                        .WithMany("GroupChatMessages")
                        .HasForeignKey("CompanyID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EnterChatWeb.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EnterChatWeb.Models.Note", b =>
                {
                    b.HasOne("EnterChatWeb.Models.Company", "Company")
                        .WithMany("Notes")
                        .HasForeignKey("CompanyID");

                    b.HasOne("EnterChatWeb.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("EnterChatWeb.Models.Topic", b =>
                {
                    b.HasOne("EnterChatWeb.Models.Company", "Company")
                        .WithMany("Topics")
                        .HasForeignKey("CompanyID");

                    b.HasOne("EnterChatWeb.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("EnterChatWeb.Models.TopicMessage", b =>
                {
                    b.HasOne("EnterChatWeb.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyID");

                    b.HasOne("EnterChatWeb.Models.Topic", "Topic")
                        .WithMany("TopicMessages")
                        .HasForeignKey("TopicID");

                    b.HasOne("EnterChatWeb.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("EnterChatWeb.Models.User", b =>
                {
                    b.HasOne("EnterChatWeb.Models.Company", "Company")
                        .WithMany("Users")
                        .HasForeignKey("CompanyID");

                    b.HasOne("EnterChatWeb.Models.Worker", "Worker")
                        .WithMany()
                        .HasForeignKey("WorkerID");
                });

            modelBuilder.Entity("EnterChatWeb.Models.Worker", b =>
                {
                    b.HasOne("EnterChatWeb.Models.Company", "Company")
                        .WithMany("Workers")
                        .HasForeignKey("CompanyID");
                });
#pragma warning restore 612, 618
        }
    }
}
