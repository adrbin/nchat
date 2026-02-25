# TODO

## Current Focus
- Unblock package restore in a network-enabled environment, then run full build and test verification.

## Blockers
- Sandbox cannot reach NuGet (`https://api.nuget.org/v3/index.json`), so restore/build/test cannot be executed in this session.

## Tasks
- [x] T-000 Create baseline docs (`PLAN.md`, `TODO.md`, `README.md`, `AGENTS.md`)
  - Status: done
  - Linked tests: n/a
  - Notes: Initial documentation established.
  - Done criteria: all docs created and aligned.

- [x] T-001 Scaffold solution structure and project files
  - Status: done
  - Linked tests: n/a
  - Notes: Added AppHost, ServiceDefaults, Web, and test projects.
  - Done criteria: all project files present and referenced from solution.

- [x] T-002 Add domain tests for value objects and entities
  - Status: done
  - Linked tests: `tests/NChat.Domain.Tests/ValueObjectsTests.cs`
  - Notes: Added tests for username, room name, message validation, and session behavior.
  - Done criteria: tests authored and mapped to acceptance criteria.

- [x] T-003 Implement domain model to satisfy tests
  - Status: done
  - Linked tests: `tests/NChat.Domain.Tests/ValueObjectsTests.cs`
  - Notes: Implemented non-anemic entities and value objects.
  - Done criteria: domain model complete.

- [x] T-004 Add application tests for session/room/message workflows
  - Status: done
  - Linked tests: `tests/NChat.Application.Tests/ChatApplicationServiceTests.cs`
  - Notes: Added service-level tests with in-memory fakes.
  - Done criteria: core use cases covered.

- [x] T-005 Implement application services and interfaces
  - Status: done
  - Linked tests: `tests/NChat.Application.Tests/ChatApplicationServiceTests.cs`
  - Notes: Added orchestration logic for sessions, rooms, messaging, and presence.
  - Done criteria: services wired and callable.

- [x] T-006 Implement infrastructure (EF Core PostgreSQL + repositories)
  - Status: done
  - Linked tests: `tests/NChat.IntegrationTests/PersistenceIntegrationTests.cs`
  - Notes: Added `NChatDbContext` and repository implementations.
  - Done criteria: persistence layer complete.

- [x] T-007 Implement HTTP APIs, SignalR hub, and Blazor pages
  - Status: done
  - Linked tests: integration/manual scenarios in `README.md`
  - Notes: Endpoints, hub methods/events, and UI flow implemented.
  - Done criteria: end-to-end flow coded.

- [ ] T-008 Execute restore/build/test and runtime verification
  - Status: blocked
  - Linked tests: all projects
  - Notes: Blocked by sandbox network resolution failure for NuGet.
  - Done criteria: `dotnet restore`, `dotnet build`, and `dotnet test` pass locally.

## Completed In This Session
- Created full solution and project structure.
- Implemented domain, application, infrastructure, API, SignalR hub, and Blazor UI.
- Added domain/application/integration test suites.
- Wired Aspire AppHost + PostgreSQL resource reference.
