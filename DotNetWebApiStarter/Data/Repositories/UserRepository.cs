using Dapper;
using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.Models;
using Microsoft.Data.SqlClient;

namespace DotNetWebApiStarter.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string? _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"SELECT Id, IdRole, Username, PasswordHash, CreatedAt
                    FROM [User]
                    WHERE Id = @Id;";

                CommandDefinition command = new CommandDefinition(query, new { Id = id }, cancellationToken: cancellationToken);
                return await connection.QueryFirstOrDefaultAsync<User>(command);
            }
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"SELECT Id, IdRole, Username, PasswordHash, CreatedAt
                    FROM [User]
                    WHERE Username = @Username;";

                CommandDefinition command = new CommandDefinition(query, new { Username = username }, cancellationToken: cancellationToken);
                return await connection.QueryFirstOrDefaultAsync<User>(command);
            }
        }

        public async Task<int> InsertAsync(User user, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"INSERT INTO [User] (IdRole, Username, PasswordHash, CreatedAt)
                    OUTPUT INSERTED.Id
                    VALUES (@IdRole, @Username, @PasswordHash, @CreatedAt);";

                CommandDefinition command = new CommandDefinition(query, user, cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<int>(command);
            }
        }

    }
}
