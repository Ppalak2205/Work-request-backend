using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Work_request.Models;

namespace Work_request.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<WorkRequest> WorkRequests => Set<WorkRequest>();
    public DbSet<WorkRequestNote> WorkRequestNotes => Set<WorkRequestNote>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WorkRequest>(entity =>
        {
            entity.Property(e => e.Priority)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasIndex(e => new { e.Status, e.CreatedDate })
                .HasDatabaseName("IX_WorkRequests_Status_CreatedDate");

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Notes)
                .WithOne(n => n.WorkRequest)
                .HasForeignKey(n => n.WorkRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<WorkRequestNote>(entity =>
        {
            entity.HasIndex(e => new { e.WorkRequestId, e.CreatedDate })
                .HasDatabaseName("IX_WorkRequestNotes_WorkRequestId_CreatedDate");

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
