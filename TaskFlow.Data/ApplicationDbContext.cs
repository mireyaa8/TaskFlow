using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data.Models;

namespace TaskFlow.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<TaskLabel> TaskLabels => Set<TaskLabel>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Entity<Project>()
            .Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Entity<Project>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.OwnedProjects)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Board>()
            .Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(80);

        builder.Entity<TaskItem>()
            .Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Entity<TaskItem>()
            .Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Entity<TaskItem>()
            .Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(30);

        builder.Entity<TaskItem>()
            .Property(t => t.Priority)
            .IsRequired()
            .HasMaxLength(20);

        builder.Entity<TaskItem>()
            .HasOne(t => t.Assignee)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Comment>()
            .Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Label>()
            .Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder.Entity<Label>()
            .Property(l => l.Color)
            .IsRequired()
            .HasMaxLength(20);

        builder.Entity<TaskLabel>()
            .HasKey(tl => new { tl.TaskItemId, tl.LabelId });

        builder.Entity<ProjectMember>()
            .HasKey(pm => new { pm.ProjectId, pm.UserId });

        builder.Entity<ProjectMember>()
            .Property(pm => pm.Role)
            .IsRequired()
            .HasMaxLength(30);
    }
}
