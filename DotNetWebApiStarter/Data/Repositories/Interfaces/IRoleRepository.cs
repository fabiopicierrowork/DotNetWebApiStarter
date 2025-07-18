using DotNetWebApiStarter.Models;

namespace DotNetWebApiStarter.Data.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}
