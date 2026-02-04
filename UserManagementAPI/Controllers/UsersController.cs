using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase

{
    private readonly IUserStore _store;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserStore store, ILogger<UsersController> logger)
    {
        _store = store;
        _logger = logger;
    }

    // GET /api/users

    // Supports pagination via ?page=1&pageSize=50 (pageSize capped at 100)
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<User>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 50 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var all = _store.GetAll();
        var total = all.Count();
        var users = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        Response.Headers.TryAdd("X-Page", page.ToString());
        Response.Headers.TryAdd("X-PageSize", pageSize.ToString());
        Response.Headers.TryAdd("X-Total-Count", total.ToString());
        Response.Headers.TryAdd("X-Count", users.Count.ToString());

        return Ok(users);
    }

    // GET /api/users/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> GetById(Guid id)
    {
        var user = _store.Get(id);
        return user is null ? NotFound() : Ok(user);
    }

    // POST /api/users

    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<User> Create([FromBody] CreateUserRequest request)
    {
        try

        {
            // Normalise inputs

            var name = (request.Name ?? string.Empty).Trim();
            var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();

            if (_store.EmailExists(email))
            {
                return Conflict(new { message = "Email already exists." });
            }

            var user = new User

            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                CreatedAt = DateTime.UtcNow

            };

            if (!_store.Add(user))
            {
                _logger.LogError("Failed to add user with id {Id}", user.Id);
                return Problem("Failed to create user.", statusCode: StatusCodes.Status500InternalServerError);
            }

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception during Create");
            return Problem("An unexpected error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // PUT /api/users/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        try

        {
            var existing = _store.Get(id);
            if (existing is null) return NotFound();

            // Normalise inputs

            var name = (request.Name ?? string.Empty).Trim();
            var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();

            if (_store.EmailExists(email, excludeUserId: id))
            {
                return Conflict(new { message = "Email already exists for another user." });
            }

            existing.Name = name;
            existing.Email = email;

            if (!_store.Update(existing))
            {
                _logger.LogError("Failed to update user with id {Id}", id);
                return Problem("Failed to update user.", statusCode: StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception during Update for {Id}", id);
            return Problem("An unexpected error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // DELETE /api/users/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        try

        {
            var existing = _store.Get(id);
            if (existing is null) return NotFound();

            if (!_store.Delete(id))
            {
                _logger.LogError("Failed to delete user with id {Id}", id);
                return Problem("Failed to delete user.", statusCode: StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception during Delete for {Id}", id);
            return Problem("An unexpected error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
