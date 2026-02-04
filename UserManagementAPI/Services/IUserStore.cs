using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public interface IUserStore

{
    IEnumerable<User> GetAll();
    User? Get(Guid id);
    bool Add(User user);
    bool Update(User user);
    bool Delete(Guid id);
    bool EmailExists(string email, Guid? excludeUserId = null);
}
