using Microsoft.EntityFrameworkCore;
using TestManager.Domain.Entities;

namespace TestManager.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Test> Tests => Set<Test>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Test>(entity =>
        {
            entity.Property(test => test.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasMany(test => test.Questions)
                .WithOne(question => question.Test)
                .HasForeignKey(question => question.TestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(question => question.Text)
                .IsRequired();

            entity.HasMany(question => question.AnswerOptions)
                .WithOne(answerOption => answerOption.Question)
                .HasForeignKey(answerOption => answerOption.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.Property(answerOption => answerOption.Text)
                .IsRequired();
        });
    }
}
