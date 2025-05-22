using Dapper;
using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.Models;
using Microsoft.Data.SqlClient;

namespace DotNetWebApiStarter.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string? _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"SELECT Id, Name, Price
                    FROM Product
                    ORDER BY Id
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY;";

                CommandDefinition command = new CommandDefinition(query, new { Offset = (pageNumber - 1) * pageSize, PageSize = pageSize }, cancellationToken: cancellationToken);
                return (await connection.QueryAsync<Product>(command));
            }
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"SELECT Id, Name, Price
                    FROM Product
                    WHERE Id = @Id;";

                CommandDefinition command = new CommandDefinition(query, new { Id = id }, cancellationToken: cancellationToken);
                return await connection.QueryFirstOrDefaultAsync<Product>(command);
            }
        }

        public async Task<int> InsertAsync(Product product, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"INSERT INTO Product (Name, Price)
                    OUTPUT INSERTED.Id
                    VALUES (@Name, @Price);";

                CommandDefinition command = new CommandDefinition(query, product, cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<int>(command);
            }
        }

        public async Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"UPDATE Product
                    SET Name = @Name,
                        Price = @Price
                    WHERE Id = @Id;";

                CommandDefinition command = new CommandDefinition(query, product, cancellationToken: cancellationToken);
                int affectedRows = await connection.ExecuteAsync(command);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string query =
                    @"DELETE FROM Product
                    WHERE Id = @Id;";

                CommandDefinition command = new CommandDefinition(query, new { Id = id }, cancellationToken: cancellationToken);
                int affectedRows = await connection.ExecuteAsync(command);
                return affectedRows > 0;
            }
        }
    }
}
