using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace MobileAPI.Services
{
    public interface IJwtAuth
    {
        string Authenticate(string member_name, string member_code, int member_id);
        JwtSecurityToken ValidateToken(HttpRequest request);
    }
    public class Auth: IJwtAuth
    {
        private readonly string key;
        private readonly string issuer;
        private readonly string audience;

        public Auth(string key, string issuer, string audience)
        {
            this.key = key;
            this.issuer = issuer;
            this.audience = audience;
        }
        public string Authenticate(string member_name,string member_code , int member_id)
        {

            try
            {
               
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenKey = Encoding.ASCII.GetBytes(key);

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(
                        new Claim[]
                        {
                        new Claim(ClaimTypes.Name, member_name),
                        new Claim(ClaimTypes.UserData, member_code),
                        new Claim(ClaimTypes.NameIdentifier, member_id.ToString())
                        }),

                    Expires = DateTime.UtcNow.AddDays(1),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public JwtSecurityToken ValidateToken(HttpRequest request)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(this.key);
                string token = request.Headers["Authorization"];
                token = token != null ? token.Split(new char[] { ' ' })[1] : "";
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = false,
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return (JwtSecurityToken)validatedToken;
            }
            catch (SecurityTokenValidationException ex)
            {
                throw new SecurityTokenValidationException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                throw new SecurityTokenValidationException(ex.Message);
            }
        }
    }
}
