using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VacationPlanner.Interfaces.Helpers;
using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Options;


namespace VacationPlanner.Implementation.Helpers
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _options;

        public JwtService(
            IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }


        public string GenerateToken(User user, Role role)
        {
            var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.UserId.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new Claim(
                ClaimTypes.Role,
                role.Name)
        };


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey));


            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _options.ExpiresMinutes),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(
                Guid.NewGuid().ToByteArray());
        }
    }
}
