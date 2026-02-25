# NChat v1 Plan

## Summary
Implement NChat with Aspire + .NET 10 + Blazor + SignalR + PostgreSQL using TDD and pragmatic DDD/Clean Architecture.

## Scope
- Session-unique usernames (active users only)
- Public room browse and create
- One active room per user
- Real-time messaging and presence via SignalR
- Persist room history in PostgreSQL
- Load latest 100 messages when entering a room

## Architecture
- `src/NChat.AppHost`: Aspire orchestration and PostgreSQL resource
- `src/NChat.ServiceDefaults`: shared telemetry/health defaults
- `src/NChat.Web`: Blazor UI + API + SignalR + application/domain/infrastructure
- `tests/*`: domain/application/integration tests

## Public APIs
- `POST /api/session/claim-name`
- `GET /api/rooms`
- `POST /api/rooms`
- `GET /api/rooms/{roomId}/presence`
- `GET /api/rooms/{roomId}/messages?take=100`

## SignalR Contract
Client -> Server:
- `SetUsername(string username)`
- `JoinRoom(Guid roomId)`
- `LeaveCurrentRoom()`
- `SendMessage(string content)`

Server -> Client:
- `RoomMessagePosted(MessageDto message)`
- `RoomPresenceChanged(RoomPresenceDto presence)`
- `RoomsChanged(RoomSummaryDto[] rooms)`
- `NameClaimResult(bool ok, string? reason)`

## DDD + Clean (Pragmatic)
- Value objects: `Username`, `RoomName`, `MessageContent`
- Behavioral entities: `UserSession`, `ChatRoom`, `ChatMessage`
- Keep boundaries clear, avoid unnecessary abstractions

## TDD Strategy
- Red-Green-Refactor per use case
- Domain tests first, then application, then integration
- Refactor only after green

## Acceptance Criteria
1. Duplicate active usernames rejected.
2. Room names are case-insensitive unique.
3. User can only be in one room at a time.
4. Messages persist and broadcast only to current room.
5. Room entry loads latest 100 chronologically.
6. Presence/counts update live on join/leave/disconnect.
7. Invalid payloads rejected consistently.

## Milestones
1. Bootstrap projects and Aspire wiring.
2. Username/session behavior and API.
3. Room lifecycle and switching.
4. Messaging + persistence.
5. Presence and live room/user updates.
6. Hardening and doc sync.

## Decision Log
- 2026-02-25: Selected PostgreSQL over MongoDB for strong relational querying and consistency in room/presence/history features.
- 2026-02-25: Username uniqueness scope is active sessions only.
- 2026-02-25: Public rooms only and one active room per user for v1.
