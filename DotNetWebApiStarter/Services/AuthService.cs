using AutoMapper;
using DotNetWebApiStarter.Contracts.Requests;
using DotNetWebApiStarter.Contracts.Responses;
using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.Models;
using DotNetWebApiStarter.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace DotNetWebApiStarter.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public AuthService(IMapper mapper, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            User? existingUser = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (existingUser is not null)
                if (existingUser.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Username already exists.");

            User newUser = _mapper.Map<User>(request);
            newUser.PasswordHash = HashPassword(request.Password);
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.Id = await _userRepository.InsertAsync(newUser, cancellationToken);

            return _mapper.Map<RegisterResponse>(newUser);
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (user is null || !VerifyPassword(request.Password, user.PasswordHash))
                return null;

            Role? role = await _roleRepository.GetByIdAsync(user.IdRole, cancellationToken);
            if(role is null)
                return null;

            string token = GenerateJwtToken(user, role);

            return new LoginResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60),
                Username = user.Username
            };
        }





        private string HashPassword(string password)
        {
            // In una vera applicazione, useresti una libreria come BCrypt.Net-CORE o ASP.NET Core Identity
            // Esempio fittizio:
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // In una vera applicazione, useresti la stessa libreria usata per l'hashing
            // Esempio fittizio:
            return HashPassword(password) == hashedPassword;
        }

        private string GenerateJwtToken(User user, Role role)
        {
            // Qui implementeresti la logica di generazione del JWT
            // richiederà configurazione (Secret Key, Issuer, Audience) e l'uso di System.IdentityModel.Tokens.Jwt
            // Esempio di payload base:
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, role.Name)
            };

            // Non è il modo corretto per generare un JWT in produzione, è solo un placeholder
            return $"FAKE_JWT_FOR_{user.Username}";
        }
    }
}
