using DotNetWebApiStarter.DTOs.Requests;
using DotNetWebApiStarter.DTOs.Responses;
using DotNetWebApiStarter.Models;

namespace DotNetWebApiStarter.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<InsertProductResponseDTO>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<InsertProductResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<InsertProductResponseDTO> InsertAsync(InsertProductRequestDTO request, CancellationToken cancellationToken);
    }
}
