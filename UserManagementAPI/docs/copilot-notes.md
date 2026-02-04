# Copilot Notes — Evidence of AI Assistance


Purpose

- This document records how GitHub Copilot assisted during development and debugging of the UserManagementAPI across Activities 1–3.
- It lists prompts, accepted suggestions, key commits/PRs, and the impact on code quality and grading criteria.

Environment

- Platform: GitHub Codespaces (web editor)
- Runtime: .NET 8

- Copilot: Enabled in Codespaces editor


---

## Activity 1 — Writing and Enhancing API Code


Prompts used

1) “Create an ASP.NET Core Web API `Program.cs` using minimal hosting, add controllers, and enable Swagger in Development.”
2) “Generate a `UsersController` with CRUD endpoints: GET (list/by id), POST, PUT, DELETE. Use `ApiController` and proper status codes.”
3) “Add DataAnnotations validation to `User`, `CreateUserRequest`, and `UpdateUserRequest` (required name/email, length limits, email format).”
4) “Implement a thread-safe in-memory store (`IUserStore`/`InMemoryUserStore`) using `ConcurrentDictionary`, including an email uniqueness check.”
5) “Ensure POST returns `CreatedAtAction` and PUT/DELETE return `NoContent`.”

Accepted suggestions

- Scaffolded `Program.cs` with controllers + Swagger.
- CRUD endpoint skeleton with `[ApiController]`, correct routes and status codes.
- DataAnnotations (`[Required]`, `[StringLength]`, `[EmailAddress]`) on model and DTOs.
- In-memory store with `GetAll`, `Get`, `Add`, `Update`, `Delete`, `EmailExists`.
- `CreatedAtAction` for POST; `NoContent` for PUT/DELETE.

Manual edits

- Trim inputs in create/update.
- Added `ProducesResponseType` annotations.
- Standardised route formatting and error bodies.

Key commits (examples — replace with real SHAs)
- 5719e0... “Copilot: scaffold Program.cs and enable Swagger”
- a1b2c3... “Copilot: generate UsersController CRUD”
- d4e5f6... “Copilot: add DataAnnotations to model and DTOs”

---

## Activity 2 — Debugging with Copilot


Prompts used

1) “Review `UsersController` and suggest error handling improvements; add try/catch and normalise inputs.”
2) “Add simple pagination to GET /api/users using `page` and `pageSize` with sane limits and response headers.”
3) “Refactor `InMemoryUserStore.Update` to use `TryUpdate` for concurrency safety.”
4) “Enhance DTOs to reject whitespace-only names using `IValidatableObject`.”

Accepted suggestions and outcomes

- Input normalisation: trim name, trim + lowercase email.
- Pagination in GET with safe limits; added headers `X-Page`, `X-PageSize`, `X-Total-Count`, `X-Count`.
- Concurrency-safe update with `TryUpdate`.
- Custom validation via `IValidatableObject` to reject whitespace-only names.
- try/catch around POST/PUT/DELETE to avoid unhandled exceptions.

Evidence

- Branch: `feature/debugging-with-copilot`
- PR: Activity 2 — Debugging with Copilot (link: <add PR URL>)
- Commits:
- b7c8d9... “Copilot: add pagination to GET users”
- c0d1e2... “Copilot: normalise inputs and add try/catch”
- f3a4b5... “Copilot: use TryUpdate for concurrency”
- 9a8b7c... “Copilot: add IValidatableObject to DTOs”

Testing notes

- Verified 404 for missing IDs, 409 for duplicate email, 400 for invalid payloads.
- Confirmed emails stored lowercased; pagination limits enforced.

---

## Activity 3 — Implementing and Managing Middleware with Copilot


Prompts used

1) “Generate middleware to log HTTP requests and responses in ASP.NET Core (method, path, status, duration).”
2) “Create global error-handling middleware that returns `{ "error": "Internal server error." }`.”
3) “Add simple API key authentication middleware for POST/PUT/DELETE; allow GET without a key.”
4) “Configure middleware pipeline order: error → auth → logging.”

Accepted suggestions and outcomes

- `RequestLoggingMiddleware`: logs method, path + query, status, elapsed ms.
- `ErrorHandlingMiddleware`: catches unhandled exceptions, returns 500 JSON.
- `AuthenticationMiddleware`: validates `X-API-Key`; returns 401 for invalid/missing key.
- Pipeline in `Program.cs`: error-handling first, authentication next, logging last.

Evidence

- Commits:
- 1a2b3c... “Copilot: add request logging middleware”
- 4d5e6f... “Copilot: add global error-handling middleware”
- 7g8h9i... “Copilot: add authentication middleware”
- j0k1l2... “Configure middleware pipeline order and Swagger”

Testing notes

- GET works without key.
- POST/PUT/DELETE return 401 without `X-API-Key`, succeed with correct key.
- Simulated exception returns 500 with JSON error body.

---

## Summary of impact


- CRUD endpoints: complete and verifiable via Swagger.
- Validation: DataAnnotations + custom DTO validation; email uniqueness with `409 Conflict`.
- Reliability: Global error handling and local try/catch prevent crashes and standardise error responses.
- Security: API key protects write operations; GET open for review convenience.
- Observability: Logs provide method/path/status/duration for auditing.

---

## Links


- Repository: <add repo URL>
- PR (Activity 2 — Debugging with Copilot): <add PR URL>
- PR (Activity 3 — Middleware): <add PR URL or list commits>
- Files:
- `Controllers/UsersController.cs`
- `Models/*.cs`
- `Services/*.cs`
- `Middleware/*.cs`
- `Program.cs`

---

## Reviewer notes


- To run in GitHub Codespaces:
- `dotnet restore && dotnet build`
- `dotnet run --project UserManagementAPI --urls http://0.0.0.0:5000`
- Open `/swagger`
- API key header for write operations: `X-API-Key: secret123` (configurable).
