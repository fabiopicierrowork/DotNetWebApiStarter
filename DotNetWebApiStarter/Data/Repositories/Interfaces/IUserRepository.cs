using DotNetWebApiStarter.Models;

namespace DotNetWebApiStarter.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<int> InsertAsync(User user, CancellationToken cancellationToken);
    }
}
