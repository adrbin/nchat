using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NChat.Web.Application.Services;
using NChat.Web.Infrastructure.Persistence;
using NChat.Web.Infrastructure.Repositories;

namespace NChat.IntegrationTests;

public sealed class PersistenceIntegrationTests
{
    [Fact]
    public async Task Message_Should_Persist_And_Be_Queried()
    {
        await using var db = CreateDb();
        var service = CreateService(db);

        await service.RegisterConnectionAsync("conn-1");
        await service.ClaimUsernameAsync("conn-1", "john");
        var room = await service.CreateRoomAsync("General", "john");
        await service.JoinRoomAsync("conn-1", room.Id);

        await service.SendMessageAsync("conn-1", "Hello world");

        var messages = await service.GetMessagesAsync(room.Id);
        messages.Should().ContainSingle();
        messages[0].Content.Should().Be("Hello world");
    }

    private static NChatDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<NChatDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new NChatDbContext(options);
    }

    private static ChatApplicationService CreateService(NChatDbContext db)
    {
        return new ChatApplicationService(
            new UserSessionRepository(db),
            new ChatRoomRepository(db),
            new ChatMessageRepository(db),
            db);
    }
}
