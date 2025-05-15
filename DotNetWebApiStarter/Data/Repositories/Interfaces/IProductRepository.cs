using DotNetWebApiStarter.Models;

namespace DotNetWebApiStarter.Data.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<int> InsertAsync(Product product, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken);
    }
}
