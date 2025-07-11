using Dapper;
using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.Models;
using Microsoft.Data.SqlClient;

namespace DotNetWebApiStarter.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string? _connectionString;

        public RoleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
        }

        public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"SELECT Id, Name
                    FROM [Role]
                    WHERE Id = @Id;";

                CommandDefinition command = new CommandDefinition(query, new { Id = id }, cancellationToken: cancellationToken);
                return await connection.QueryFirstOrDefaultAsync<Role>(command);
            }
        }

    }
}
