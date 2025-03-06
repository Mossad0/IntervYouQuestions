using System;
using System.Collections.Generic;
using IntervYouQuestions.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntervYouQuestions.Api.Persistence;

public partial class InterviewModuleContext : DbContext
{
    public InterviewModuleContext()
    {
    }

    public InterviewModuleContext(DbContextOptions<InterviewModuleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ModelAnswer> ModelAnswers { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=db11765.public.databaseasp.net; Database=db11765; User Id=db11765; Password=oT?9K8s_3m=F; Encrypt=False; MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BE68A9E3C");

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Weight).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<ModelAnswer>(entity =>
        {
            entity.HasKey(e => e.ModelAnswerId).HasName("PK__Model_An__DFF72A5B619179FE");

            entity.ToTable("Model_Answers");

            entity.Property(e => e.ModelAnswerId).HasColumnName("Model_Answer_Id");
            entity.Property(e => e.KeyPoints).HasColumnName("Key_Points");
            entity.Property(e => e.Text).HasColumnType("text");

            entity.HasOne(d => d.Question).WithMany(p => p.ModelAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__Model_Ans__Quest__412EB0B6");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06FACD726E932");

            entity.Property(e => e.Difficulty).HasMaxLength(100);
            entity.Property(e => e.Text).HasColumnType("text");
            entity.Property(e => e.Type).HasMaxLength(200);

            entity.HasOne(d => d.Topic).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK__Questions__Topic__3E52440B");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.OptionId).HasName("PK__Question__3260907EADE631F9");

            entity.ToTable("Question_Options");

            entity.Property(e => e.OptionId).HasColumnName("Option_Id");
            entity.Property(e => e.IsCorrect).HasColumnName("isCorrect");
            entity.Property(e => e.Text).HasColumnType("text");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionOptions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Question___Quest__440B1D61");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__Topics__022E0F5D533AA597");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Topics)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Topics__Category__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
