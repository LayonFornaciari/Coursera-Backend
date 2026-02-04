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
        if (!_users.ContainsKey(user.Id)) return false;
        _users[user.Id] = user;
        return true;
    }

    public bool Delete(Guid id) => _users.TryRemove(id, out _);

    public bool EmailExists(string email, Guid? excludeUserId = null)
    {
        return _users.Values.Any(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
            (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
    }
}
