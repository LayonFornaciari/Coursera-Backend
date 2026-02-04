# UserManagementAPI


A simple User Management REST API built with ASP.NET Core (.NET 8). This repository is the submission for the “Peer‑graded Assignment: Project: Building a Simple API with Copilot”.

The API supports TechHive Solutions’ internal HR/IT needs with create, read, update, and delete (CRUD) user operations. Across three activities, the project evolves from scaffolding and CRUD to debugging and robust middleware (logging, error handling, and authentication).

## Overview


- Framework: ASP.NET Core Web API (minimal hosting, .NET 8)
- Store: In-memory, thread-safe `ConcurrentDictionary`
- Tooling: GitHub (web UI + Codespaces), GitHub Copilot

- Authentication: Simple API key middleware for write operations

- Documentation/Testing: Swagger (Development), curl/Postman


## Quick start (GitHub Codespaces)

1) Open a Codespace on this repo.
2) In the terminal:
- `dotnet restore`
- `dotnet build`
- `dotnet run --project UserManagementAPI --urls http://0.0.0.0:5000`
3) Open the forwarded URL and browse to `/swagger`.

Environment/config:
- API key in `appsettings.json` at `ApiSecurity:ApiKey` (default: `secret123`)
- Can be overridden via environment variable: `ApiSecurity__ApiKey=yourKey`

## API endpoints


Base URL: `/api/users`

- GET `/api/users`
- Returns a paged list of users. Query parameters: `page` (default 1), `pageSize` (default 50, max 100).
- Response headers: `X-Page`, `X-PageSize`, `X-Total-Count`, `X-Count`.
- GET `/api/users/{id}`
- Returns a single user by ID.
- POST `/api/users` (requires API key)
- Creates a new user. Returns 201 with `Location` header.
- PUT `/api/users/{id}` (requires API key)
- Updates an existing user. Returns 204.
- DELETE `/api/users/{id}` (requires API key)
- Deletes a user. Returns 204.

Status codes in use:
- 200 OK, 201 Created, 204 No Content, 400 Bad Request, 401 Unauthorized, 404 Not Found, 409 Conflict, 500 Internal Server Error.

## Authentication


- Write operations (POST/PUT/DELETE) require an API key.
- Header: `X-API-Key: secret123` (or your configured value).
- Read operations (GET) do not require a key to ease peer review.

## Validation rules


- Name:
- Required, 2–100 characters.
- Must contain non‑whitespace characters.
- Email:
- Required, valid email format, max 254 characters.
- Normalised to lowercase and trimmed.
- Uniqueness:
- Email must be unique. Duplicate email attempts return `409 Conflict`.
- Error responses:
- Invalid payload -> `400 Bad Request` via `[ApiController]` and DataAnnotations.
- Non-existent ID -> `404 Not Found`.
- Conflicts (duplicate email) -> `409 Conflict`.

Relevant files:
- `UserManagementAPI/Models/User.cs`
- `UserManagementAPI/Models/CreateUserRequest.cs`
- `UserManagementAPI/Models/UpdateUserRequest.cs`
- `UserManagementAPI/Controllers/UsersController.cs`

## Middleware


- Global error handling (`UserManagementAPI/Middleware/ErrorHandlingMiddleware.cs`)
- Catches unhandled exceptions and returns:
- Status: `500 Internal Server Error`
- Body: `{ "error": "Internal server error." }`
- Authentication (`UserManagementAPI/Middleware/AuthenticationMiddleware.cs`)
- Validates `X-API-Key` for POST/PUT/DELETE.
- Missing/invalid key -> `401 Unauthorized` with `{ "error": "API key missing or invalid." }`
- Request logging (`UserManagementAPI/Middleware/RequestLoggingMiddleware.cs`)
- Logs method, path (with query), response status, and duration in ms.

Pipeline order (in `Program.cs`):
1) Error handling
2) Authentication
3) Request logging
Then Swagger (Development) and controllers.

## Testing the API


Run in Codespaces:
- `dotnet restore && dotnet build`
- `dotnet run --project UserManagementAPI --urls http://0.0.0.0:5000`
- Open `/swagger` to exercise endpoints.

Example curl:
- GET list (no key):
- `curl http://localhost:5000/api/users`
- POST (requires key):
- `curl -X POST http://localhost:5000/api/users -H "Content-Type: application/json" -H "X-API-Key: secret123" -d "{\"name\":\"Alice\",\"email\":\"alice@example.com\"}"`
- PUT (requires key):
- `curl -X PUT http://localhost:5000/api/users/{id} -H "Content-Type: application/json" -H "X-API-Key: secret123" -d "{\"name\":\"Alice B\",\"email\":\"alice.b@example.com\"}"`
- DELETE (requires key):
- `curl -X DELETE http://localhost:5000/api/users/{id} -H "X-API-Key: secret123"`

Expected outcomes:
- GET 200; POST 201 with Location; GET by id 200/404; PUT 204/400/409; DELETE 204/404; unauthenticated write -> 401.

## Activities and Copilot usage


### Activity 1 — Writing and Enhancing API Code

- Implemented CRUD endpoints, DTOs, and validation with Copilot-assisted scaffolding.
- Accepted improvements: `ProducesResponseType` annotations, trimming inputs, RESTful status codes.
- See “Copilot usage — Activity 1” section below for prompts and outcomes.

### Activity 2 — Debugging with Copilot

- Fixed validation gaps (reject whitespace-only names via `IValidatableObject`).
- Normalised email handling (trim + lowercase) for reliable uniqueness checks.
- Added pagination to GET with safe limits and paging headers.
- Hardened update concurrency in store (`TryUpdate`).
- Added try/catch around write operations (complemented by global error middleware in Activity 3).
- Evidence:
- PR: Activity 2 — Debugging with Copilot (link your PR)
- Notes: `docs/copilot-notes.md`

### Activity 3 — Implementing and Managing Middleware with Copilot

- Added global error handling, authentication (API key), and request logging middleware.
- Configured pipeline order: error → auth → logging.
- Documented how to test auth and error handling via Swagger/curl.
- Evidence:
- Commits referencing Copilot

- Notes: `docs/copilot-notes.md`

## Copilot usage — Activity 1


Overview
I enabled GitHub Copilot in a Codespace and used it to scaffold the API, generate CRUD endpoints, and suggest validation rules. I reviewed each suggestion, accepted the ones that met the project goals, and made small manual edits for consistency and clarity.

Prompts used and outcomes
- “Create an ASP.NET Core Web API `Program.cs` using minimal hosting, add controllers, and enable Swagger in Development.”
Outcome: Copilot scaffolded service registration, `MapControllers`, and Swagger setup.
- “Generate a `UsersController` with CRUD endpoints: GET (list/by id), POST, PUT, DELETE. Use `ApiController` and return proper status codes.”
Outcome: Controller skeleton with routes and consistent responses (`200`/`201`/`204`/`404`/`409`).
- “Add DataAnnotations validation to `User`, `CreateUserRequest`, and `UpdateUserRequest` (required name/email, length limits, email format).”
Outcome: `[Required]`, `[StringLength]`, `[EmailAddress]`, enabling automatic `400` responses via `[ApiController]`.
- “Implement a thread‑safe in‑memory user store (`IUserStore`/`InMemoryUserStore`) using `ConcurrentDictionary`, including an email uniqueness check.”
Outcome: Interface and implementation with `GetAll`, `Get`, `Add`, `Update`, `Delete`, `EmailExists`.
- “Ensure POST returns `CreatedAtAction` and PUT/DELETE return `NoContent` when successful.”
Outcome: Updated return types and status codes to align with REST conventions.

Enhancements accepted
- Trim input on create/update; `ProducesResponseType` annotations; `409 Conflict` for duplicates; `404 NotFound` for missing IDs; move storage logic to `Services`.

Manual edits I made
- Standardised routes and error bodies; aligned names and namespaces with repo structure.

## Grading evidence (25 pts total)

- [x] GitHub repository created (5 pts)
- Public repository URL: <add URL here>
- Clear structure and README.
- [x] CRUD endpoints implemented (5 pts)
- See `UserManagementAPI/Controllers/UsersController.cs`
- Endpoints listed above; verifiable in Swagger.
- [x] Used Copilot to debug the code (5 pts)
- Branch: `feature/debugging-with-copilot`
- PR: Activity 2 — Debugging with Copilot (link)
- Notes: `docs/copilot-notes.md` (prompts and accepted suggestions)
- [x] Validation to process only valid user data (5 pts)
- DataAnnotations on DTOs/models; `IValidatableObject`; duplicate email check with `409 Conflict`.
- [x] Middleware implemented (5 pts)
- Error handling, authentication, and logging middleware with correct pipeline order; documented testing steps.

## Project structure

- `UserManagementAPI/Controllers/UsersController.cs`
- `UserManagementAPI/Models/*`
- `UserManagementAPI/Services/*`
- `UserManagementAPI/Middleware/*`
- `UserManagementAPI/Program.cs`
- `UserManagementAPI/appsettings.json`
- `UserManagementAPI/UserManagementAPI.csproj`
- `README.md`
- `docs/copilot-notes.md` (evidence)

## Notes

- For real projects, keep secrets out of source control; prefer environment variables or secret stores.
- The in-memory store is for demonstration; swap with EF Core for persistence.
