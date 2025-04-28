using AutoMapper;
using DotNetWebApiStarter.Contracts.Requests;
using DotNetWebApiStarter.Contracts.Responses;
using DotNetWebApiStarter.Data.Repositories.Interfaces;
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

        public async Task<IEnumerable<CreateProductResponse>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<Product> products = await _productRepository.GetAllAsync(pageNumber, pageSize, cancellationToken);
            IEnumerable<CreateProductResponse> response = _mapper.Map<IEnumerable<CreateProductResponse>>(products);

            return response;
        }

        public async Task<CreateProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Product? product = await _productRepository.GetByIdAsync(id, cancellationToken);
            CreateProductResponse response = _mapper.Map<CreateProductResponse>(product);

            return response;
        }

        public async Task<CreateProductResponse> InsertAsync(CreateProductRequest request, CancellationToken cancellationToken)
        {
            Product product = _mapper.Map<Product>(request);
            int productId = await _productRepository.InsertAsync(product, cancellationToken);
            product.Id = productId;
            CreateProductResponse response = _mapper.Map<CreateProductResponse>(product);

            return response;
        }
    }
}
