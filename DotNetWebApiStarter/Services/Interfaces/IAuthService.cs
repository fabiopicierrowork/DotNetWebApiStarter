using DotNetWebApiStarter.Contracts.Requests;
using DotNetWebApiStarter.Contracts.Responses;

namespace DotNetWebApiStarter.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    }
}
