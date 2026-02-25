# AGENTS

## Engineering Contract

### TDD is required
- Every behavior change starts with a failing test.
- Use Red-Green-Refactor.
- Prefer domain tests first, then application, then integration.

### DDD + Clean Architecture (pragmatic)
- Keep domain behavior in entities/value objects when it adds clarity.
- Avoid intentional anemic entities.
- Keep boundaries clear:
  - `Domain`: core rules and behavior
  - `Application`: use cases/contracts
  - `Infrastructure`: persistence and external adapters
  - `Presentation`: APIs, SignalR hub, Blazor UI
- Do not over-engineer; avoid abstractions with no current value.

### Documentation policy
- Update `TODO.md` status when starting/completing tasks.
- Update `README.md` when user-facing behavior or run steps change.
- Update `PLAN.md` only for approved scope/design decisions and record in Decision Log.

### Definition of done
A task is done only when:
1. Code is implemented.
2. Tests exist and pass locally.
3. `TODO.md` is updated.
4. `README.md` changes are applied when relevant.
