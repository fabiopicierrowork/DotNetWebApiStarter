using AutoMapper;
using DotNetWebApiStarter.DTOs.Requests;
using DotNetWebApiStarter.DTOs.Responses;
using DotNetWebApiStarter.Models;

namespace DotNetWebApiStarter.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<InsertProductRequestDTO, Product>();
            CreateMap<Product, InsertProductResponseDTO>();
        }
    }
}
