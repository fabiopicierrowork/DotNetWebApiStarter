using AutoMapper;
using DotNetWebApiStarter.Contracts.Responses;
using DotNetWebApiStarter.Models;

namespace DotNetWebApiStarter.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, GetUserResponse>();
        }
    }
}
