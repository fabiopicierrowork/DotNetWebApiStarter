using DotNetWebApiStarter.Contracts.Requests;
using DotNetWebApiStarter.Contracts.Responses;

namespace DotNetWebApiStarter.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<CreateProductResponse>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<CreateProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CreateProductResponse> InsertAsync(CreateProductRequest request, CancellationToken cancellationToken);
    }
}
