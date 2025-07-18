using AutoMapper;
using DotNetWebApiStarter.Contracts.Requests;
using DotNetWebApiStarter.Contracts.Responses;
using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.Models;
using DotNetWebApiStarter.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotNetWebApiStarter.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            User? existingUser = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (existingUser is not null)
                if (existingUser.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Username already exists.");

            User newUser = _mapper.Map<User>(request);
            newUser.PasswordHash = _HashPassword(request.Password);
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.Id = await _userRepository.InsertAsync(newUser, cancellationToken);

            return _mapper.Map<RegisterResponse>(newUser);
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request, string clientIpAddress, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (user is null || !_VerifyPassword(request.Password, user.PasswordHash))
                return null;

            Role? role = await _roleRepository.GetByIdAsync(user.IdRole, cancellationToken);
            if(role is null)
                return null;

            await _RevokeAllActiveRefreshTokensForUser(user.Id, clientIpAddress, cancellationToken);

            string accessToken = _GenerateJwtToken(user, role);
            int accessTokenExpiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes", 60);
            DateTime accessTokenExpiration = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes);

            RefreshToken refreshToken = await _GenerateAndStoreRefreshToken(user.Id, clientIpAddress, cancellationToken);

            return new LoginResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresAt
            };
        }

        public async Task<LoginResponse?> RefreshTokenAsync(string refreshTokenString, string clientIpAddress, CancellationToken cancellationToken)
        {
            RefreshToken? refreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenString, cancellationToken);

            if (refreshToken is null || refreshToken.IsRevoked)
            {
                if (refreshToken is not null)
                    await _RevokeAllActiveRefreshTokensForUser(refreshToken.IdUser, clientIpAddress, cancellationToken);

                return null;
            }

            if (refreshToken.IsExpired)
                return null;

            User? user = await _userRepository.GetByIdAsync(refreshToken.IdUser, cancellationToken);
            if (user is null)
            {
                refreshToken.RevokedAt = DateTime.UtcNow;
                refreshToken.RevokedByIp = clientIpAddress;
                await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
                return null;
            }

            Role? role = await _roleRepository.GetByIdAsync(user.IdRole, cancellationToken);
            if (role is null)
            {
                refreshToken.RevokedAt = DateTime.UtcNow;
                refreshToken.RevokedByIp = clientIpAddress;
                await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
                return null;
            }

            string newAccessToken = _GenerateJwtToken(user, role);
            int accessTokenExpiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes", 60);
            DateTime newAccessTokenExpiration = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes);

            RefreshToken newRefreshToken = await _GenerateAndStoreRefreshToken(user.Id, clientIpAddress, cancellationToken);

            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = clientIpAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

            return new LoginResponse
            {
                AccessToken = newAccessToken,
                AccessTokenExpiration = newAccessTokenExpiration,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresAt
            };
        }

        private string _HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        private bool _VerifyPassword(string password, string hashedPassword)
        {
            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }

        private string _GenerateJwtToken(User user, Role role)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, role.Name)
            };

            string? jwtKey = _configuration["Jwt:Key"];
            string? jwtIssuer = _configuration["Jwt:Issuer"];
            string? jwtAudience = _configuration["Jwt:Audience"];
            int expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes", 60);

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
                throw new InvalidOperationException("JWT configuration is missing in appsettings.json.");

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RefreshToken> _GenerateAndStoreRefreshToken(int idUser, string clientIpAddress, CancellationToken cancellationToken)
        {
            byte[] randomNumber = new byte[32];
            using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
                randomNumberGenerator.GetBytes(randomNumber);
            string refreshTokenString = Convert.ToBase64String(randomNumber);

            int refreshTokenExpiryDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpiryDays", 7);
            DateTime refreshTokenExpiration = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

            RefreshToken refreshToken = new RefreshToken
            {
                IdUser = idUser,
                Token = refreshTokenString,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = clientIpAddress,
                ExpiresAt = refreshTokenExpiration
            };

            await _refreshTokenRepository.InsertAsync(refreshToken, cancellationToken);

            return refreshToken;
        }

        private async Task _RevokeAllActiveRefreshTokensForUser(int idUser, string clientIpAddress, CancellationToken cancellationToken)
        {
            IEnumerable<RefreshToken> activeRefreshTokens = await _refreshTokenRepository.GetAllActiveByUserIdAsync(idUser, cancellationToken);

            foreach (RefreshToken token in activeRefreshTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = clientIpAddress;
                await _refreshTokenRepository.UpdateAsync(token, cancellationToken);
            }
        }
    }
}
