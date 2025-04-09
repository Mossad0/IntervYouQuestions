using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using IntervYouQuestions.Api.Authentication;
using Microsoft.Extensions.Options;
using IntervYouQuestions.Api.Authentication.Dto;


namespace IntervYouQuestions.Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;

        public UserService(
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _configuration = configuration;
            _jwtSettings = jwtSettings.Value;

        }


        public JwtTokenResult GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var jti = Guid.NewGuid().ToString();

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, jti),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.GivenName, user.FullName)
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenValidityInMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = creds,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new JwtTokenResult
            {
                Token = tokenString,
                Jti = jti,
                ExpiresAt = expires
            };
        }

        public Task<List<Interview>> GetUserInterviewsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserStatsResponse> GetUserStatsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, 
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private double CalculateInterviewScore(Interview interview)
        {
            if (interview.Status != InterviewStatus.Completed || !interview.InterviewQuestions.Any())
            {
                return 0;
            }

            var correctAnswers = interview.InterviewQuestions.Count(iq => iq.Question.QuestionOptions.Any(qo => qo.IsCorrect));
            return (double)correctAnswers / interview.InterviewQuestions.Count * 100;
        }

    }

}

