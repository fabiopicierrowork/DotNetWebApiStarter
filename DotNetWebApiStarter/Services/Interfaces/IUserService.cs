using DotNetWebApiStarter.Contracts.Responses;

namespace DotNetWebApiStarter.Services.Interfaces
{
    public interface IUserService
    {
        Task<GetUserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}
