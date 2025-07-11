using AutoMapper;
using DotNetWebApiStarter.Contracts.Requests;
using DotNetWebApiStarter.Contracts.Responses;
using DotNetWebApiStarter.Models;

namespace DotNetWebApiStarter.Mappings
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterRequest, User>();
            CreateMap<User, RegisterResponse>()
                .ForMember(dest => dest.IdUser, opt => opt.MapFrom(src => src.Id));
        }
    }
}
