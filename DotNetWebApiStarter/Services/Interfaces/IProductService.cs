using DotNetWebApiStarter.Contracts.Requests;
using DotNetWebApiStarter.Contracts.Responses;

namespace DotNetWebApiStarter.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<GetProductResponse>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<GetProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CreateProductResponse> InsertAsync(CreateProductRequest request, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(UpdateProductRequest request, CancellationToken cancellationToken);
    }
}
