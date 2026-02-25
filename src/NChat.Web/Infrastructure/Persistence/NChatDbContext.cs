using Microsoft.EntityFrameworkCore;
using NChat.Web.Application.Abstractions;
using NChat.Web.Domain.Entities;

namespace NChat.Web.Infrastructure.Persistence;

public sealed class NChatDbContext : DbContext, IUnitOfWork
{
    public NChatDbContext(DbContextOptions<NChatDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();

    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.ToTable("users_session");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ConnectionId).HasColumnName("connection_id");
            entity.Property(x => x.UsernameValue).HasColumnName("username");
            entity.Property(x => x.CurrentRoomId).HasColumnName("current_room_id");
            entity.Property(x => x.IsActive).HasColumnName("is_active");
            entity.Property(x => x.ConnectedAt).HasColumnName("connected_at");
            entity.Property(x => x.LastSeenAt).HasColumnName("last_seen_at");

            entity.HasIndex(x => x.ConnectionId).IsUnique();
            entity.HasIndex(x => x.UsernameValue)
                .IsUnique()
                .HasFilter("is_active = true AND username IS NOT NULL");
        });

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.ToTable("chat_rooms");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.NameValue).HasColumnName("name");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at");
            entity.Property(x => x.CreatedByUsername).HasColumnName("created_by_username");
            entity.HasIndex(x => x.NameValue).IsUnique();
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("chat_messages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RoomId).HasColumnName("room_id");
            entity.Property(x => x.Username).HasColumnName("username");
            entity.Property(x => x.Content).HasColumnName("content");
            entity.Property(x => x.SentAt).HasColumnName("sent_at");
            entity.HasIndex(x => new { x.RoomId, x.SentAt });
        });
    }

    async Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        _ = await base.SaveChangesAsync(cancellationToken);
    }
}
