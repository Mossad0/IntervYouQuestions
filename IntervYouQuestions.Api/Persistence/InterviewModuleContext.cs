using Microsoft.EntityFrameworkCore;

namespace IntervYouQuestions.Api.Persistence
{
    public partial class InterviewModuleContext : DbContext
    {
        public InterviewModuleContext()
        {
        }

        public InterviewModuleContext(DbContextOptions<InterviewModuleContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ModelAnswer> ModelAnswers { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionOption> QuestionOptions { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }

        public DbSet<InterviewQuestion> InterviewQuestions { get; set; }

        public DbSet<UserAnswer> UserAnswers { get; set; }

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

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__Users__UserId");
                entity.Property(e => e.UserId).HasMaxLength(50);
                entity.Property(e => e.Username).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property (e => e.Password).HasMaxLength(500).IsRequired(); // Store hashed password
                entity.Property(e => e.ExperienceLevel).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
                entity.Property(e => e.RegisteredDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasIndex(e => e.Email).IsUnique();
            });

            // UserProfile Entity
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.ProfileId).HasName("PK__UserProfiles");
                entity.Property(e => e.FullName).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.LinkedInProfile).HasMaxLength(500);
                entity.Property(e => e.GitHubProfile).HasMaxLength(500);
                entity.Property(e => e.Biography).HasColumnType("text");

                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<UserProfile>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserProfile__User");
            });

            // Interview Entity
            modelBuilder.Entity<Interview>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Interviews");
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ExpirationDate).HasColumnType("datetime").IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.ExperienceLevel).IsRequired();
                entity.Property(e => e.StartTime).HasColumnType("datetime").IsRequired();
                entity.Property(e => e.Role).HasMaxLength(100).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Interviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Interviews__User");
            });

            // InterviewQuestion Entity (Many-to-Many Relationship)
            modelBuilder.Entity<InterviewQuestion>(entity =>
            {
                entity.ToTable("InterviewQuestion"); // Explicitly set the table name
                entity.HasKey(e => e.Id).HasName("PK__InterviewQuestions");
                entity.Property(e => e.OrderIndex).IsRequired();
                entity.HasOne(d => d.Interview)
                    .WithMany(p => p.InterviewQuestions)
                    .HasForeignKey(d => d.InterviewId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__InterviewQuestions__Interview");
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.InterviewQuestions)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__InterviewQuestions__Question");
            });

            modelBuilder.Entity<UserAnswer>()
            .HasOne(ua => ua.Interview)
            .WithMany()
            .HasForeignKey(ua => ua.InterviewId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany()
                .HasForeignKey(ua => ua.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);



            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}