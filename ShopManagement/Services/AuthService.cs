using Microsoft.IdentityModel.Tokens;
using ShopManagement.Interfaces;
using ShopManagement.Models.DTOs;
using ShopManagement.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShopManagement.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string userId);
    }

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if user already exists
            var existingUser = (await _unitOfWork.Users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
            if (existingUser != null)
                throw new ArgumentException("User with this email already exists.");

            // Create new user
            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                ShopName = request.ShopName,
                Phone = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            // Generate tokens
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.CompleteAsync();

            return new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                ShopName = user.ShopName,
                Phone = user.Phone,
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Email and password are required.");
            }

            var emailToFind = request.Email.Trim().ToLower();

            try
            {

                var user = (await _unitOfWork.Users.FindAsync(u => 
                    u.Email != null && u.Email.Trim().ToLower() == emailToFind))
                    .FirstOrDefault();

                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                if (!VerifyPassword(request.Password, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                if (user.IsActive == 0)
                {
                    throw new UnauthorizedAccessException("Account is deactivated.");
                }

                // Generate tokens
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Save refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.LastLoginAt = DateTime.UtcNow; // Optional: track last login
                
                await _unitOfWork.CompleteAsync();

                return new AuthResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    ShopName = user.ShopName,
                    Phone = user.Phone,
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpiry = DateTime.UtcNow.AddHours(1)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var user = (await _unitOfWork.Users.FindAsync(u => u.RefreshToken == refreshToken)).FirstOrDefault();
            if (user == null || user.RefreshTokenExpiryTime.Value <= DateTime.UtcNow){
                throw new SecurityTokenException("Invalid refresh token.");
            }
            // Generate new tokens
            var newToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Update refresh token
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.CompleteAsync();

            return new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                ShopName = user.ShopName,
                Phone = user.Phone,
                Token = newToken,
                RefreshToken = newRefreshToken,
                TokenExpiry = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task<bool> RevokeTokenAsync(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("ShopName", user.ShopName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
