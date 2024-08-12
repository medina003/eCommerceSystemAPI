using eCommerceRESTfulAPI.Application.Interfaces.Hashing;
using eCommerceRESTfulAPI.Domain.Entities;
using eCommerceRESTfulAPI.Domain.Interfaces.Repositories;
using eCommerceRESTfulAPI.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Localization; 
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceRESTfulAPI.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IStringLocalizer<AuthService> _localizer; 

        public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings, IPasswordHasherService passwordHasher, IStringLocalizer<AuthService> localizer) // Добавьте localizer в параметры
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
            _passwordHasher = passwordHasher;
            _localizer = localizer;
        }

        public async Task<string> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password))
            {
                throw new UnauthorizedAccessException(_localizer["InvalidEmailOrPassword"]);
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
