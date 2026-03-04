using Microsoft.EntityFrameworkCore;
using NChat.Web.Application.Abstractions;
using NChat.Web.Application.Dto;
using NChat.Web.Application.Services;
using NChat.Web.Components;
using NChat.Web.Domain.Exceptions;
using NChat.Web.Infrastructure.Persistence;
using NChat.Web.Infrastructure.Repositories;
using NChat.Web.Presentation.Contracts;
using NChat.Web.Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();

var connectionString = builder.Configuration.GetConnectionString("nchatdb");

builder.Services.AddDbContext<NChatDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<NChatDbContext>());
builder.Services.AddScoped<ChatApplicationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NChatDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapHub<ChatHub>("/hubs/chat");

var api = app.MapGroup("/api");

api.MapPost("/session/claim-name", async Task<IResult>(
    ClaimNameRequest request,
    HttpContext httpContext,
    ChatApplicationService service,
    CancellationToken cancellationToken) =>
{
    var connectionId = httpContext.Request.Headers["x-connection-id"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(connectionId))
    {
        return TypedResults.BadRequest(new { reason = "Missing x-connection-id header." });
    }

    ClaimUsernameResult result;
    try
    {
        result = await service.ClaimUsernameAsync(connectionId, request.Username, cancellationToken);
    }
    catch (DomainValidationException ex)
    {
        return TypedResults.BadRequest(new { reason = ex.Message });
    }

    if (!result.Ok)
    {
        return TypedResults.BadRequest(new { reason = result.Reason });
    }

    return TypedResults.Ok(new { ok = true, normalizedUsername = result.NormalizedUsername });
});

api.MapGet("/rooms", async (ChatApplicationService service, CancellationToken cancellationToken) =>
{
    var rooms = await service.ListRoomsAsync(cancellationToken);
    return TypedResults.Ok(rooms);
});

api.MapPost("/rooms", async Task<IResult>(
    CreateRoomRequest request,
    HttpContext httpContext,
    ChatApplicationService service,
    CancellationToken cancellationToken) =>
{
    var username = httpContext.Request.Headers["x-username"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(username))
    {
        return TypedResults.BadRequest(new { reason = "Missing x-username header." });
    }

    try
    {
        var room = await service.CreateRoomAsync(request.Name, username, cancellationToken);
        return TypedResults.Ok(new { room.Id, Name = room.Name, room.UserCount });
    }
    catch (DomainValidationException ex)
    {
        return TypedResults.BadRequest(new { reason = ex.Message });
    }
});

api.MapGet("/rooms/{roomId:guid}/presence", async (Guid roomId, ChatApplicationService service, CancellationToken cancellationToken) =>
{
    var presence = await service.GetPresenceAsync(roomId, cancellationToken);
    return TypedResults.Ok(presence);
});

api.MapGet("/rooms/{roomId:guid}/messages", async (Guid roomId, int? take, ChatApplicationService service, CancellationToken cancellationToken) =>
{
    var messages = await service.GetMessagesAsync(roomId, take ?? 100, cancellationToken);
    return TypedResults.Ok(messages);
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public partial class Program;
