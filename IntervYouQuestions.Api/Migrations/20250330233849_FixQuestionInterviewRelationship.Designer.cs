﻿// <auto-generated />
using System;
using IntervYouQuestions.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IntervYouQuestions.Api.Migrations
{
    [DbContext(typeof(InterviewModuleContext))]
    [Migration("20250330233849_FixQuestionInterviewRelationship")]
    partial class FixQuestionInterviewRelationship
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(18, 0)");

                    b.HasKey("CategoryId")
                        .HasName("PK__Categori__19093A0BE68A9E3C");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Interview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("ExperienceLevel")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id")
                        .HasName("PK__Interviews");

                    b.HasIndex("UserId");

                    b.ToTable("Interviews");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.InterviewQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("InterviewId")
                        .HasColumnType("int");

                    b.Property<int>("OrderIndex")
                        .HasColumnType("int");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__InterviewQuestions");

                    b.HasIndex("InterviewId");

                    b.HasIndex("QuestionId");

                    b.ToTable("InterviewQuestion");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.ModelAnswer", b =>
                {
                    b.Property<int>("ModelAnswerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Model_Answer_Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ModelAnswerId"));

                    b.Property<string>("KeyPoints")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Key_Points");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ModelAnswerId")
                        .HasName("PK__Model_An__DFF72A5B619179FE");

                    b.HasIndex("QuestionId");

                    b.ToTable("Model_Answers", (string)null);
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Question", b =>
                {
                    b.Property<int>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuestionId"));

                    b.Property<string>("Difficulty")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TopicId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("QuestionId")
                        .HasName("PK__Question__0DC06FACD726E932");

                    b.HasIndex("TopicId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.QuestionOption", b =>
                {
                    b.Property<int>("OptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Option_Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OptionId"));

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit")
                        .HasColumnName("isCorrect");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("OptionId")
                        .HasName("PK__Question__3260907EADE631F9");

                    b.HasIndex("QuestionId");

                    b.ToTable("Question_Options", (string)null);
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Topic", b =>
                {
                    b.Property<int>("TopicId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TopicId"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("TopicId")
                        .HasName("PK__Topics__022E0F5D533AA597");

                    b.HasIndex("CategoryId");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ExperienceLevel")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("RegisteredDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("UserId")
                        .HasName("PK__Users__UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.UserProfile", b =>
                {
                    b.Property<int>("ProfileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProfileId"));

                    b.Property<double>("AverageScore")
                        .HasColumnType("float");

                    b.Property<string>("Biography")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("GitHubProfile")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("LinkedInProfile")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("TotalInterviewsCompleted")
                        .HasColumnType("int");

                    b.Property<int>("TotalInterviewsTaken")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ProfileId")
                        .HasName("PK__UserProfiles");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserProfile");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Interview", b =>
                {
                    b.HasOne("IntervYouQuestions.Api.Entities.User", "User")
                        .WithMany("Interviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Interviews__User");

                    b.Navigation("User");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.InterviewQuestion", b =>
                {
                    b.HasOne("IntervYouQuestions.Api.Entities.Interview", "Interview")
                        .WithMany("InterviewQuestions")
                        .HasForeignKey("InterviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__InterviewQuestions__Interview");

                    b.HasOne("IntervYouQuestions.Api.Entities.Question", "Question")
                        .WithMany("InterviewQuestions")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__InterviewQuestions__Question");

                    b.Navigation("Interview");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.ModelAnswer", b =>
                {
                    b.HasOne("IntervYouQuestions.Api.Entities.Question", "Question")
                        .WithMany("ModelAnswers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Model_Ans__Quest__412EB0B6");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Question", b =>
                {
                    b.HasOne("IntervYouQuestions.Api.Entities.Topic", "Topic")
                        .WithMany("Questions")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Questions__Topic__3E52440B");

                    b.Navigation("Topic");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.QuestionOption", b =>
                {
                    b.HasOne("IntervYouQuestions.Api.Entities.Question", "Question")
                        .WithMany("QuestionOptions")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Question___Quest__440B1D61");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Topic", b =>
                {
                    b.HasOne("IntervYouQuestions.Api.Entities.Category", "Category")
                        .WithMany("Topics")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Topics__Category__3B75D760");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.UserProfile", b =>
                {
                    b.HasOne("IntervYouQuestions.Api.Entities.User", "User")
                        .WithOne()
                        .HasForeignKey("IntervYouQuestions.Api.Entities.UserProfile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__UserProfile__User");

                    b.Navigation("User");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Category", b =>
                {
                    b.Navigation("Topics");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Interview", b =>
                {
                    b.Navigation("InterviewQuestions");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Question", b =>
                {
                    b.Navigation("InterviewQuestions");

                    b.Navigation("ModelAnswers");

                    b.Navigation("QuestionOptions");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.Topic", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("IntervYouQuestions.Api.Entities.User", b =>
                {
                    b.Navigation("Interviews");
                });
#pragma warning restore 612, 618
        }
    }
}
