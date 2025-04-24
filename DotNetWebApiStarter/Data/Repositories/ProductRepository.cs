using Dapper;
using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.Models;
using System.Data;
using static Dapper.SqlMapper;
using System.Data.Common;

namespace DotNetWebApiStarter.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _dbContext;

        public ProductRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (IDbConnection connection = await _dbContext.GetConnectionAsync())
            {
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
            using (IDbConnection connection = await _dbContext.GetConnectionAsync())
            {
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
            using (IDbConnection connection = await _dbContext.GetConnectionAsync())
            {
                string query =
                    @"INSERT INTO Product (Name, Price)
                    OUTPUT INSERTED.Id
                    VALUES (@Name, @Price);";

                CommandDefinition command = new CommandDefinition(query, product, cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<int>(command);
            }
        }
    }
}
