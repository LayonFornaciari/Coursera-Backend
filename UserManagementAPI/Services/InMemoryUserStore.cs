using System.Collections.Concurrent;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class InMemoryUserStore : IUserStore

{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public IEnumerable<User> GetAll() => _users.Values.OrderBy(u => u.CreatedAt);

    public User? Get(Guid id) => _users.TryGetValue(id, out var user) ? user : null;

    public bool Add(User user) => _users.TryAdd(user.Id, user);

    public bool Update(User user)
    {
        // Use TryUpdate for concurrency safety

        if (!_users.TryGetValue(user.Id, out var current))
            return false;

        return _users.TryUpdate(user.Id, user, current);
    }

    public bool Delete(Guid id) => _users.TryRemove(id, out _);

    public bool EmailExists(string email, Guid? excludeUserId = null)
    {
        var normalised = email.Trim().ToLowerInvariant();
        return _users.Values.Any(u =>
            u.Email.Equals(normalised, StringComparison.Ordinal) &&
            (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
    }
}
