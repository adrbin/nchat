# NChat

NChat is a pragmatic real-time chat application built with Aspire, .NET 10, Blazor, SignalR, and PostgreSQL.

## Features
- Session-unique usernames (active sessions only)
- Public room browse/create
- One active room per user
- Live messaging and room presence updates
- Persisted room history in PostgreSQL
- Latest 100 messages loaded on room entry

## Architecture Snapshot
- `src/NChat.AppHost`: Aspire orchestration and PostgreSQL resource
- `src/NChat.ServiceDefaults`: shared service defaults extension point
- `src/NChat.Web`: Blazor UI + REST API + SignalR + pragmatic DDD/Clean layers
- `tests/NChat.Domain.Tests`: domain rules and invariants
- `tests/NChat.Application.Tests`: use-case orchestration tests
- `tests/NChat.IntegrationTests`: persistence integration tests

## Prerequisites
- .NET SDK `10.0.103` or compatible .NET 10 SDK
- Docker Desktop (for PostgreSQL under Aspire)
- NuGet network access to restore packages

## Run
1. Restore packages:
   - `dotnet restore NChat.sln`
2. Run Aspire AppHost:
   - `dotnet run --project src/NChat.AppHost`
3. Open the Aspire dashboard and start the `web` project.

## Test
- `dotnet test NChat.sln`

## Progress Tracking
Current progress and blockers are tracked in [`TODO.md`](TODO.md).

## Manual Verification Scenarios
1. Open two browser sessions and claim the same username; second attempt should be rejected.
2. Create two rooms with same name different casing; second should fail.
3. Join room A, then room B; user should appear only in room B.
4. Send message in room; it should broadcast in real time and persist.
5. Refresh and rejoin room; latest 100 messages should load in chronological order.

## Known Limitations (v1)
- No permanent authentication provider
- Public rooms only
- Single-instance assumption (no SignalR backplane)
