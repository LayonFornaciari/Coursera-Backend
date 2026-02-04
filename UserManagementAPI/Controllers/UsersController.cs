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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<User>> GetAll()
    {
        return Ok(_store.GetAll());
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
        // [ApiController] auto-validates ModelState using DataAnnotations

        if (_store.EmailExists(request.Email))
        {
            return Conflict(new { message = "Email already exists." });
        }

        var user = new User

        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            CreatedAt = DateTime.UtcNow

        };

        if (!_store.Add(user))
        {
            _logger.LogError("Failed to add user with id {Id}", user.Id);
            return Problem("Failed to create user.", statusCode: StatusCodes.Status500InternalServerError);
        }

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // PUT /api/users/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var existing = _store.Get(id);
        if (existing is null) return NotFound();

        if (_store.EmailExists(request.Email, excludeUserId: id))
        {
            return Conflict(new { message = "Email already exists for another user." });
        }

        existing.Name = request.Name.Trim();
        existing.Email = request.Email.Trim();

        if (!_store.Update(existing))
        {
            _logger.LogError("Failed to update user with id {Id}", id);
            return Problem("Failed to update user.", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    // DELETE /api/users/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
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
}
