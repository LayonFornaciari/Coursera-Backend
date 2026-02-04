# UserManagementAPI

A simple User Management REST API built with ASP.NET Core and developed with the assistance of GitHub Copilot. This repository is the submission for the “Peer‑graded Assignment: Project: Building a Simple API with Copilot”.

The API is designed to support HR and IT teams at TechHive Solutions with basic user operations: create, read, update, and delete (CRUD). Across three activities, the project evolves from scaffolding and CRUD to debugging and robust middleware (logging, error handling, and token-based authentication).

## Project Goals

- Build a working ASP.NET Core Web API (targeting .NET 8).
- Implement full CRUD operations for user entities:

- GET: list users and retrieve a user by ID
- POST: create a new user
- PUT: update an existing user
- DELETE: remove a user by ID

- Use GitHub Copilot to:
- Scaffold boilerplate code
- Enhance endpoints and validation
- Debug issues and improve reliability
- Add data validation to ensure only valid user data is processed (e.g., required fields, email format, string lengths, and uniqueness rules).
- Implement middleware to:
- Log requests and responses for auditing
- Provide consistent, centralised error handling
- Secure endpoints with token‑based authentication
- Document how Copilot assisted during development and debugging (prompts, suggestions, and commits).
- Provide simple testing instructions for reviewers (Swagger and/or curl/Postman).

## Activities and Milestones

- Activity 1: Writing and Enhancing API Code
- Scaffold project structure and implement CRUD endpoints.
- Apply basic validation using DataAnnotations.

- Activity 2: Debugging API Code with Copilot
- Identify and fix issues (validation gaps, error handling, 404 cases, and minor performance tweaks) with Copilot’s help.
- Document bugs found and how Copilot assisted.

- Activity 3: Implementing and Managing Middleware with Copilot
- Add logging middleware (method, path, status code, duration).
- Add error‑handling middleware (consistent JSON error responses).
- Add authentication middleware (token/API key validation).
- Configure middleware pipeline order as required.

## Grading Criteria (25 pts total)

This section will be updated with evidence as the project progresses.

1) GitHub repository created (5 pts)
- Status: [ ] Not started [ ] In progress [x] Complete
- Evidence:
- Public repository URL: <add URL here>
- Initial README committed

2) CRUD endpoints implemented (GET, POST, PUT, DELETE) (5 pts)
- Status: [ ] Not started [ ] In progress [ ] Complete
- Evidence (to add):
- Controller name and file path:
- Example endpoint list:
- GET /api/users
- GET /api/users/{id}
- POST /api/users
- PUT /api/users/{id}
- DELETE /api/users/{id}
- Screenshot or notes from Swagger/Postman tests

3) Used Copilot to debug the code (5 pts)
- Status: [ ] Not started [ ] In progress [ ] Complete
- Evidence (to add):
- Prompts used and outcomes (link to docs/copilot-notes.md)
- Commit(s)/PR(s) referencing Copilot suggestions
- Summary of bugs identified and fixes applied

4) Additional functionality: validation to process only valid user data (5 pts)
- Status: [ ] Not started [ ] In progress [ ] Complete
- Evidence (to add):
- Validation rules (required fields, email format, string lengths)
- Duplicate email/uniqueness handling
- HTTP responses for invalid data (e.g., 400/409)
- Links to DTOs/models with DataAnnotations

5) Middleware implemented (logging and/or authentication) (5 pts)
- Status: [ ] Not started [ ] In progress [ ] Complete
- Evidence (to add):
- Logging middleware file path and behaviour (method, path, status, duration)
- Error‑handling middleware file path and example JSON error payload
- Authentication middleware file path and how to provide token/API key
- Pipeline order shown in `Program.cs`

## How This Repo Will Be Worked On

- Development will occur directly on GitHub (web editor and/or Codespaces).
- Branching strategy:
- feature/activity-1-crud
- feature/activity-2-debugging
- feature/activity-3-middleware
- Pull Requests will summarise Copilot’s contributions and link to issues when applicable.
- Commit messages will explicitly reference Copilot where suggestions were used (e.g., “Copilot: add 404 handling for GET by id”).

- Ensure repository is public and update the Grading Criteria evidence.
- Submit the repository URL for peer review.

## Copilot usage — Activity 1


### Overview

I enabled GitHub Copilot in a Codespace and used it to scaffold the API, generate CRUD endpoints, and suggest validation rules. I reviewed each suggestion, accepted the ones that met the project goals, and made small manual edits for consistency and clarity.

### Prompts used and outcomes

- “Create an ASP.NET Core Web API `Program.cs` using minimal hosting, add controllers, and enable Swagger in Development.”
- Outcome: Copilot scaffolded service registration, `MapControllers`, and Swagger setup.
- “Generate a `UsersController` with CRUD endpoints: GET (list/by id), POST, PUT, DELETE. Use `ApiController` and return proper status codes.”
- Outcome: Copilot produced the controller skeleton with routes, consistent responses (`200`/`201`/`204`/`404`/`409`), and example payload shapes.
- “Add DataAnnotations validation to `User`, `CreateUserRequest`, and `UpdateUserRequest` (required name/email, length limits, email format).”
- Outcome: Copilot added `[Required]`, `[StringLength]`, and `[EmailAddress]` guidance, enabling automatic `400 Bad Request` responses via `[ApiController]`.
- “Implement a thread‑safe in‑memory user store (`IUserStore`/`InMemoryUserStore`) using `ConcurrentDictionary`, including an email uniqueness check.”
- Outcome: Copilot proposed the interface and a simple store with `GetAll`, `Get`, `Add`, `Update`, `Delete`, and `EmailExists`.
- “Ensure POST returns `CreatedAtAction` and PUT/DELETE return `NoContent` when successful.”
- Outcome: Copilot updated return types and status codes to align with REST conventions.

### Enhancements accepted

- Trim input fields on create/update to avoid whitespace issues.
- Use `ProducesResponseType` annotations to document responses in Swagger.
- Return `409 Conflict` when an email is already taken, both on create and update.
- Return `404 NotFound` when a user id does not exist.
- Keep controllers lean and move storage logic into `Services`.

### Manual edits I made

- Standardised route formatting and response bodies for conflicts and errors.
- Clarified model/DTO names and file layout to match the repository structure.
- Ensured consistent naming and casing across namespaces and folders.

## Activity 2 — Bugs Found and Fixed (Copilot-assisted)

- Validation gaps:
- Added `IValidatableObject` to DTOs to reject whitespace-only names.
- Normalised emails (trim + lowercase) for reliable uniqueness checks.

- Robust status codes:
- Confirmed `404 NotFound` for missing IDs.
- `409 Conflict` on duplicate emails for create/update.
- `400 Bad Request` for invalid DTOs via `[ApiController]`.

- Exception handling:
- Wrapped create/update/delete in try/catch; return `Problem(500)` on unhandled exceptions.
- (Global error handling will be implemented as middleware in Activity 3.)

- Performance/UX:
- Added pagination to `GET /api/users` via `?page=&pageSize=` with safe limits.
- Exposed pagination info in response headers: `X-Page`, `X-PageSize`, `X-Count`.

- Evidence:
- See `docs/copilot-notes.md` and the PR “Activity 2 — Debugging with Copilot”.
