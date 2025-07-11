using AutoMapper;
using DotNetWebApiStarter.Contracts.Responses;
using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.Models;
using DotNetWebApiStarter.Services.Interfaces;

namespace DotNetWebApiStarter.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserService(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<GetUserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetByIdAsync(id, cancellationToken);
            GetUserResponse response = _mapper.Map<GetUserResponse>(user);

            return response;
        }

    }
}
