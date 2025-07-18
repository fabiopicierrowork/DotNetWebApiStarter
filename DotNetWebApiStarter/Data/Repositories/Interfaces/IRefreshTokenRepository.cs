using DotNetWebApiStarter.Models;

namespace DotNetWebApiStarter.Data.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task<IEnumerable<RefreshToken>> GetAllActiveByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<int> InsertAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    }
}
