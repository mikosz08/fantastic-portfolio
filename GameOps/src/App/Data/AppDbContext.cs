using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace App.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var e = modelBuilder.Entity<Event>();
        e.ToTable("events");

        e.HasKey(x => x.Id);

        e.Property(x => x.Id).HasColumnName("id");

        e.Property(x => x.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();

        e.Property(x => x.Type)
            .HasColumnName("type")
            .HasMaxLength(100)
            .IsRequired();

        e.Property(x => x.PlayerId)
            .HasColumnName("player_id")
            .HasMaxLength(64);

        e.Property(x => x.SessionId)
            .HasColumnName("session_id")
            .HasMaxLength(64);

        e.Property(x => x.IdempotencyKey)
            .HasColumnName("idempotency_key")
            .HasMaxLength(128)
            .IsRequired();

        // jsonb payload
        e.Property(x => x.Payload)
            .HasColumnName("payload")
            .HasColumnType("jsonb");

        // default now() at UTC on DB side
        e.Property(x => x.IngestedAt)
            .HasColumnName("ingested_at")
            .HasDefaultValueSql("now() at time zone 'utc'")
            .ValueGeneratedOnAdd();

        // unique idempotency guard
        e.HasIndex(x => x.IdempotencyKey)
            .IsUnique()
            .HasDatabaseName("ux_events_idempotency_key");
    }
}
