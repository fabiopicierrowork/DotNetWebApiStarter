using Dapper;
using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.Models;
using Microsoft.Data.SqlClient;

namespace DotNetWebApiStarter.Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly string? _connectionString;

        public RefreshTokenRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"SELECT Id, IdUser, Token, CreatedAt, CreatedByIp, ExpiresAt, RevokedAt, RevokedByIp
                  FROM [RefreshToken]
                  WHERE Token = @Token;";

                CommandDefinition command = new CommandDefinition(query, new { Token = token }, cancellationToken: cancellationToken);
                return await connection.QueryFirstOrDefaultAsync<RefreshToken>(command);
            }
        }

        public async Task<IEnumerable<RefreshToken>> GetAllActiveByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"SELECT Id, IdUser, Token, CreatedAt, CreatedByIp, ExpiresAt, RevokedAt, RevokedByIp
                    FROM [RefreshToken]
                    WHERE IdUser = @IdUser AND RevokedAt IS NULL AND ExpiresAt > GETUTCDATE();";

                CommandDefinition command = new CommandDefinition(query, new { IdUser = userId }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<RefreshToken>(command);
            }
        }

        public async Task<int> InsertAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"INSERT INTO [RefreshToken] (IdUser, Token, CreatedAt, CreatedByIp, ExpiresAt)
                    OUTPUT INSERTED.Id
                    VALUES (@IdUser, @Token, @CreatedAt, @CreatedByIp, @ExpiresAt);";

                CommandDefinition command = new CommandDefinition(query, refreshToken, cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<int>(command);
            }
        }

        public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"UPDATE [RefreshToken]
                    SET IdUser = @IdUser,
                        Token = @Token,
                        CreatedAt = @CreatedAt,
                        CreatedByIp = @CreatedByIp,
                        ExpiresAt = @ExpiresAt,
                        RevokedAt = @RevokedAt,
                        RevokedByIp = @RevokedByIp,
                        ReplacedByToken = @ReplacedByToken
                    WHERE Id = @Id;";

                CommandDefinition command = new CommandDefinition(query, refreshToken, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }
    }
}
