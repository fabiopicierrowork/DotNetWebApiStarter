using AutoMapper;
using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.DTOs.Requests;
using DotNetWebApiStarter.DTOs.Responses;
using DotNetWebApiStarter.Models;
using DotNetWebApiStarter.Services.Interfaces;

namespace DotNetWebApiStarter.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductService(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<InsertProductResponseDTO>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<Product> products = await _productRepository.GetAllAsync(pageNumber, pageSize, cancellationToken);
            IEnumerable<InsertProductResponseDTO> response = _mapper.Map<IEnumerable<InsertProductResponseDTO>>(products);

            return response;
        }

        public async Task<InsertProductResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Product? product = await _productRepository.GetByIdAsync(id, cancellationToken);
            InsertProductResponseDTO response = _mapper.Map<InsertProductResponseDTO>(product);

            return response;
        }

        public async Task<InsertProductResponseDTO> InsertAsync(InsertProductRequestDTO request, CancellationToken cancellationToken)
        {
            Product product = _mapper.Map<Product>(request);
            int productId = await _productRepository.InsertAsync(product, cancellationToken);
            product.Id = productId;
            InsertProductResponseDTO response = _mapper.Map<InsertProductResponseDTO>(product);

            return response;
        }
    }
}
