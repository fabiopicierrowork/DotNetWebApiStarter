using AutoMapper;
using DotNetWebApiStarter.Contracts.Requests;
using DotNetWebApiStarter.Contracts.Responses;
using DotNetWebApiStarter.Models;

namespace DotNetWebApiStarter.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductRequest, Product>();
            CreateMap<Product, GetProductResponse>();
            CreateMap<Product, CreateProductResponse>();
        }
    }
}
